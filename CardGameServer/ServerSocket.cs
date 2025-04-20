using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks.Dataflow;

namespace CardGameServer
{
    class ServerSocket
    {
        public Socket socket;
        private Dictionary<int, ClientSocket> clientDic = new Dictionary<int, ClientSocket>();
        private Dictionary<int, PlayerInfo> playerInfoDic = new Dictionary<int, PlayerInfo>();
        // 添加这两个集合用于跟踪已设置属性的客户端
        private HashSet<int> clientSetHP = new HashSet<int>();
        private HashSet<int> clientSetAtk = new HashSet<int>();
        public int clientNum = 0;
        private bool isClose;
        public static List<CardData> deck;
        private bool isInitRound; // 是否是初始化回合
        private bool isGameRound; // 是否是游戏回合
        public class CardData
        {
            public int value;
            public string prefabName;
        }
        //抽牌方法
        public static CardData DrawCard()
        {
            if (deck.Count > 0)
            {
                CardData card = deck[0];
                deck.RemoveAt(0);
                return card;
            }
            else
            {
                Debug.WriteLine("没有牌了！");
                ReshuffleDeck(); // 重新洗牌
                if (deck.Count > 0)
                {
                    CardData card = deck[0];
                    deck.RemoveAt(0);
                    return card;
                }
                return null;
            }
        }
        private static void ReshuffleDeck()
        {
            deck = new List<CardData>();
            string[] suits = new string[] { "Heart", "Club", "Diamond", "Spade" };
            // 为 2~14 生成各花色牌，共 4*13 =52 张
            foreach (string suit in suits)
            {
                for (int i = 2; i <= 14; i++)
                {
                    deck.Add(new CardData { value = i, prefabName = suit + "_" + i });
                }
            }
            // 添加两张Joker（大小王）
            deck.Add(new CardData { value = 15, prefabName = "Joker_Small" });
            deck.Add(new CardData { value = 15, prefabName = "Joker_Big" });
            // 洗牌
            Random random = new Random();
            for (int i = 0; i < deck.Count; i++)
            {
                int rnd = random.Next(i, deck.Count);
                CardData temp = deck[i];
                deck[i] = deck[rnd];
                deck[rnd] = temp;
            }
            
            Console.WriteLine("重新洗牌完成，牌堆数量: " + deck.Count);
        }
        public void ResetGame()
        {
            deck = new List<CardData>();
            string[] suits = new string[] { "Heart", "Club", "Diamond", "Spade" };
            // 为 2~14 生成各花色牌，共 4*13 =52 张
            foreach (string suit in suits)
            {
                for (int i = 2; i <= 14; i++)
                {
                    deck.Add(new CardData { value = i, prefabName = suit + "_" + i });
                }
            }
            // 添加两张Joker（大小王）
            deck.Add(new CardData { value = 15, prefabName = "Joker_Small" });
            deck.Add(new CardData { value = 15, prefabName = "Joker_Big" });
            // 洗牌
            Random random = new Random();
            for (int i = 0; i < deck.Count; i++)
            {
                int rnd = random.Next(i, deck.Count);
                CardData temp = deck[i];
                deck[i] = deck[rnd];
                deck[rnd] = temp;
            }
            // 当四个人都连接成功后，开始游戏
            if (clientDic.Count == 4)
            {
                // 玩家初始状态修改：等待玩家选择
                PlayerInfo newPlayer1 = new PlayerInfo();
                newPlayer1.clientId = clientDic.ElementAt(0).Key;
                newPlayer1.playerName = "Player1";
                newPlayer1.HP = 0;
                newPlayer1.Atk = 0;
                newPlayer1.isDefence = false;
                newPlayer1.isDead = newPlayer1.HP <= 0;
                newPlayer1.HpCardName = "0"; // 这个阶段没有卡牌
                newPlayer1.AtkCardName = "0"; // 这个阶段没有卡牌
                PlayerInfo newPlayer2 = new PlayerInfo();
                newPlayer2.clientId = clientDic.ElementAt(1).Key;
                newPlayer2.playerName = "Player2";
                newPlayer2.HP = 0;
                newPlayer2.Atk = 0;
                newPlayer2.isDefence = false;
                newPlayer2.isDead = newPlayer2.HP <= 0;
                newPlayer2.HpCardName = "0";
                newPlayer2.AtkCardName = "0"; // 这个阶段没有卡牌
                PlayerInfo newPlayer3 = new PlayerInfo();
                newPlayer3.clientId = clientDic.ElementAt(2).Key;
                newPlayer3.playerName = "Player3";
                newPlayer3.HP = 0;
                newPlayer3.Atk = 0;
                newPlayer3.isDefence = false;
                newPlayer3.isDead = newPlayer3.HP <= 0;
                newPlayer3.HpCardName = "0";
                newPlayer3.AtkCardName = "0"; // 这个阶段没有卡牌
                PlayerInfo newPlayer4 = new PlayerInfo();
                newPlayer4.clientId = clientDic.ElementAt(3).Key;
                newPlayer4.playerName = "Player4";
                newPlayer4.HP = 0;
                newPlayer4.Atk = 0;
                newPlayer4.isDefence = false;
                newPlayer4.isDead = newPlayer4.HP <= 0;
                newPlayer4.HpCardName = "0";
                newPlayer4.AtkCardName = "0"; // 这个阶段没有卡牌
                isInitRound = true; // 初始化回合
                isGameRound = false; // 游戏回合
                clientSetHP.Clear(); // 清空已设置生命值的集合
                clientSetAtk.Clear(); // 清空已设置攻击力的集合
                // 发送第一张牌给每个玩家
                SendCard(newPlayer1.clientId);
                SendCard(newPlayer2.clientId);
                SendCard(newPlayer3.clientId);
                SendCard(newPlayer4.clientId);
                // 随机先手
                List<PlayerInfo> playerList = new List<PlayerInfo> { newPlayer1, newPlayer2, newPlayer3, newPlayer4 };
                Random rand = new Random();
                playerList = playerList.OrderBy(p => rand.Next()).ToList();
                playerList[0].isMyTurn = true;
                for (int i = 1; i < playerList.Count; i++)
                {
                    playerList[i].isMyTurn = false;
                }
                playerInfoDic.Clear();
                foreach (var player in playerList)
                {
                    playerInfoDic.Add(player.clientId, player);
                }
                // 广播游戏开始消息
                StartGameMsg startMsg = new StartGameMsg();
                startMsg.gameStarted = true;
                foreach (ClientSocket client in clientDic.Values.ToList())
                {
                    client.SendMsg(startMsg);
                }
                BroadcastPlayerInfo();
            }
        }
        //方法 给对应客户端发送一张牌
        private void SendCard(int clientId)
        {
            CardData card = DrawCard();
            if (card != null)
            {
                DrawCardMsg msg = new DrawCardMsg();
                msg.clientId = clientId;
                msg.cardValue = card.value;
                msg.cardPrefabName = card.prefabName;
                // 发送给对应客户端
                if (clientDic.ContainsKey(clientId))
                {
                    clientDic[clientId].SendMsg(msg);
                    Console.WriteLine("发送牌成功，ID：" + clientId + " 牌值：" + card.value + " 预设体名称：" + card.prefabName);
                }
            }
        }
        public void UpdatePlayerHPInfo(int clientId, int HP, string HpCardName)
        {
            if (playerInfoDic.ContainsKey(clientId))
            {
                playerInfoDic[clientId].HP = HP;
                playerInfoDic[clientId].isDead = playerInfoDic[clientId].HP <= 0;
                playerInfoDic[clientId].HpCardName = HpCardName; // 更新卡牌名称
                if (isInitRound && !isGameRound)
                {
                    clientSetHP.Add(clientId); // 添加到已设置生命值的集合中
                    CheckInitializationComplete(); // 检查初始化是否完成
                }
                // 广播玩家信息
                BroadcastPlayerInfo();
                // 如果当前是初始化回合，给玩家发送一张牌
                if (isInitRound)
                {
                    SendCard(clientId); // 给玩家发送一张牌
                }
            }
        }
        public void UpdatePlayerAtkInfo(int clientId, int Atk, string AtkCardName)
        {
            if (playerInfoDic.ContainsKey(clientId))
            {
                playerInfoDic[clientId].Atk = Atk;
                playerInfoDic[clientId].AtkCardName = AtkCardName; // 更新卡牌名称
                if (isInitRound && !isGameRound)
                {
                    clientSetAtk.Add(clientId); // 添加到已设置攻击力的集合中
                    CheckInitializationComplete(); // 检查初始化是否完成
                }
                // 广播玩家信息
                BroadcastPlayerInfo();
                // 如果当前是初始化回合，给玩家发送一张牌
                if (isInitRound)
                {
                    SendCard(clientId); // 给玩家发送一张牌
                }
            }
        }
        private void CheckInitializationComplete()
        {
            if (!isInitRound || isGameRound) return; // 如果不是初始化回合或已经开始游戏，则不检查
            if (clientSetHP.Count == clientNum && clientSetAtk.Count == clientNum)
            {
                Console.WriteLine("所有玩家的生命值和攻击力都已设置，开始游戏！");
                isInitRound = false; // 设置为游戏回合
                isGameRound = true; // 设置为游戏回合
                // 清空
                clientSetHP.Clear();
                clientSetAtk.Clear();
                // 广播游戏开始消息
                BroadcastPlayerInfo();
            }
        }
        // 更新玩家防御状态
        public void UpdatePlayerIsDefenceInfo(int clientId, bool isDefence)
        {
            if (playerInfoDic.ContainsKey(clientId))
            {
                playerInfoDic[clientId].isDefence = isDefence;
                BroadcastPlayerInfo();
            }
        }
        // 玩家攻击其他玩家
        public void PlayerAttack(int clientId, int targetClientId)
        {
            if (playerInfoDic.ContainsKey(clientId) && playerInfoDic.ContainsKey(targetClientId))
            {
                if (playerInfoDic[targetClientId].isDefence)
                {
                    // 处于防御状态 受伤害减半
                    playerInfoDic[targetClientId].HP -= playerInfoDic[clientId].Atk / 2;
                    BroadcastPlayerInfo();
                }
                else
                {
                    playerInfoDic[targetClientId].HP -= playerInfoDic[clientId].Atk;
                    BroadcastPlayerInfo();
                }
                if (playerInfoDic.TryGetValue(clientId, out var attacker) && playerInfoDic.TryGetValue(targetClientId, out var target))
                {
                    AttackResultMsg attackMsg = new AttackResultMsg
                    {
                        clientId = targetClientId,
                        value = target.HP,
                        isHp = true
                    };
                    clientDic[clientId].SendMsg(attackMsg); // 发送攻击结果给目标玩家
                    AttackResultMsg targetResultMsg = new AttackResultMsg
                    {
                        clientId = clientId,
                        value = attacker.Atk,
                        isHp = false
                    };
                    clientDic[targetClientId].SendMsg(targetResultMsg); // 发送攻击结果给攻击者
                }
            }
        }
        // 获取下一个玩家ID 返回下一个玩家ID
        private int GetNextClientId(int nowClientId)
        {
            int nextClientId = nowClientId + 1;
            if (nextClientId > clientDic.ElementAt(clientDic.Count - 1).Key)
            {
                nextClientId = clientDic.ElementAt(0).Key; // 获取第一个玩家ID
            }
            // 检查下一个玩家ID是否存在于字典中且存活
            if (clientDic.ContainsKey(nextClientId) && playerInfoDic[nextClientId].HP > 0)
            {
                return nextClientId;
            }
            else
            {
                return GetNextClientId(nextClientId); // 递归查找下一个玩家ID
            }
        }
        // 下一个玩家轮次
        public void NextPlayerTurn(int nowClientId, int nextClientId, bool isEndTurn)
        {
            if (isEndTurn)
            {
                // 结束当前玩家回合
                if (playerInfoDic.ContainsKey(nowClientId))
                {
                    playerInfoDic[nowClientId].isMyTurn = false;
                }
                // 先确定下一个玩家ID
                nextClientId = GetNextClientId(nowClientId);
                // 开始下一个玩家回合
                if (playerInfoDic.ContainsKey(nextClientId))
                {
                    playerInfoDic[nextClientId].isMyTurn = true;
                    BroadcastPlayerInfo();
                    SendCard(nextClientId); // 给下一个玩家发送一张牌
                }
            }
        }
        public void StartServer(string ip, int port, int num)
        {
            isClose = false;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            socket.Bind(iPEndPoint);
            socket.Listen(num);
            Console.WriteLine("服务器开启成功");
            ThreadPool.QueueUserWorkItem(AcceptClient);
            ThreadPool.QueueUserWorkItem(ReceiveClientMsg);
        }
    
        private void AcceptClient(object obj)
        {
            while (!isClose)
            {
                try
                {
                    Socket clientSocket = socket.Accept();
                    ClientSocket client = new ClientSocket(clientSocket);
                    clientDic.Add(client.clientId, client);
                    clientNum++;
                    Console.WriteLine("客户端连接成功，ID：" + client.clientId);
                    // 连接成功后发送客户端ID信息
                    ClientIdMsg msg = new ClientIdMsg();
                    msg.clientId = client.clientId;
                    client.SendMsg(msg);
                }
                catch (Exception e)
                {
                    Console.WriteLine("接收客户端连接出错" + e.Message);
                }
            }
        }
        public void CloseServer()
        {
            isClose = true;
            foreach (ClientSocket client in clientDic.Values)
            {
                client.Close();
            }
            clientDic.Clear();
            if (socket != null)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                socket = null;
            }
            Console.WriteLine("服务器关闭成功");
        }
        //接收客户端消息
        private void ReceiveClientMsg(object obj)
        {
            bool gameStarted = false;
            while (!isClose)
            {
                lock (clientDic)
                {
                    // 接收所有客户端消息
                    foreach (ClientSocket client in clientDic.Values.ToList())
                    {
                        client.ReceiveMsg();
                    }
                    // 根据在线客户端数量判断游戏开始或结束状态
                    if (clientDic.Count >= 4 && !gameStarted)
                    {
                        gameStarted = true;
                        ResetGame();
                    }
                    else if (clientDic.Count < 4 && gameStarted)
                    {
                        gameStarted = false;
                        // 广播游戏未开始消息
                        StartGameMsg startMsg = new StartGameMsg();
                        startMsg.gameStarted = false;
                        foreach (ClientSocket client in clientDic.Values.ToList())
                        {
                            client.SendMsg(startMsg);
                        }
                    }
                    // 判断游戏是否结束（只有一个客户端生命值大于0）
                    int aliveCount = 0;
                    // 如果现在是游戏阶段
                    if (gameStarted && isGameRound)
                    {
                        foreach (PlayerInfo playerInfo in playerInfoDic.Values.ToList())
                        {
                            if (playerInfo.HP > 0)
                            {
                                aliveCount++;
                            }
                        }
                        if (aliveCount <= 1)
                        {
                            // 游戏结束 广播游戏结束消息
                            GameOverMsg gameOverMsg = new GameOverMsg();
                            gameOverMsg.isGameOver = true;
                            // 找到胜利者 血量大于0的玩家
                            foreach (PlayerInfo playerInfo in playerInfoDic.Values.ToList())
                            {
                                if (playerInfo.HP > 0)
                                {
                                    gameOverMsg.winnerId = playerInfo.clientId;
                                    break;
                                }
                            }
                            foreach (ClientSocket client in clientDic.Values.ToList())
                            {
                                client.SendMsg(gameOverMsg);
                            }
                        }
                    }
                }
                // 避免高频循环，降低CPU占用
                Thread.Sleep(50);
            }
        }
        //处理客户端断开连接 服务器不知道的问题
        public void CloseClientSocket(ClientSocket socket)
        {
            lock (clientDic)
            {
                socket.Close();
                if (clientDic.ContainsKey(socket.clientId))
                {
                    clientDic.Remove(socket.clientId);
                    clientNum--;
                    Console.WriteLine("客户端断开连接，ID：" + socket.clientId);
                }
                if (playerInfoDic.ContainsKey(socket.clientId))
                {
                    playerInfoDic.Remove(socket.clientId);
                    Console.WriteLine("玩家信息删除成功，ID：" + socket.clientId);
                }
            }
        }
        //广播玩家信息
        private void BroadcastPlayerInfo()
        {
            //给客户端发送所有玩家信息
            foreach (ClientSocket client in clientDic.Values.ToList())
            {
                foreach (PlayerInfo playerInfo in playerInfoDic.Values.ToList())
                {
                    PlayerInfoMsg msg = new PlayerInfoMsg();
                    msg.isInitRound = isInitRound;
                    msg.isGameRound = isGameRound;
                    msg.playerInfo1 = playerInfoDic[clientDic.ElementAt(0).Key];
                    msg.playerInfo2 = playerInfoDic[clientDic.ElementAt(1).Key];
                    msg.playerInfo3 = playerInfoDic[clientDic.ElementAt(2).Key];
                    msg.playerInfo4 = playerInfoDic[clientDic.ElementAt(3).Key];
                    client.SendMsg(msg);
                }
                Console.WriteLine("广播玩家信息成功，ID：" + client.clientId);
            }
        }
    }
}