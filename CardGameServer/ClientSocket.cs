using System.Net.Sockets;

namespace CardGameServer
{
    class ClientSocket
    {
        private static int CLIENT_BEGIN_ID = 0;
        public Socket socket;
        public int clientId;
        public ClientSocket(Socket socket)
        {
            this.socket = socket;
            this.clientId = CLIENT_BEGIN_ID;
            this.clientId = Interlocked.Increment(ref CLIENT_BEGIN_ID);
        }
        public void Close()
        {
            if (socket != null)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                socket = null;
            }
        }
        public void SendMsg(BaseMsg info)
        {
            if (socket.Connected)
            {
                try
                {
                    socket.Send(info.Writing());
                }
                catch (Exception e)
                {
                    Console.WriteLine("发消息出错" + e.Message);
                }
            }
        }
        public void ReceiveMsg()
        {
            if (!socket.Connected || (socket.Poll(10, SelectMode.SelectRead) && socket.Available == 0))
            {
                Program.serverSocket.CloseClientSocket(this);
                return;
            }
            try
            {
                if (socket.Available > 0)
                {
                    byte[] result = new byte[1024 * 1024];
                    int receiveNum = socket.Receive(result);
                    HandleReceiveMsg(result, receiveNum);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("接收消息出错" + e.Message);
            }
        }
        private void HandleReceiveMsg(byte[] receiveBytes, int receiveNum)
        {
            int msgID = 0;
            int nowIndex = 0;
            int msgBodyLength = 0;
            while (nowIndex < receiveNum)
            {
                if (receiveNum > 0)
                {
                    msgID = BitConverter.ToInt32(receiveBytes, nowIndex);
                    nowIndex += 4;
                    msgBodyLength = BitConverter.ToInt32(receiveBytes, nowIndex);
                    nowIndex += 4;
                    BaseMsg baseMsg = null;
                    switch (msgID)
                    {
                        case 1001:
                            baseMsg = new ClientIdMsg();
                            baseMsg.Reading(receiveBytes, nowIndex);
                            break;
                        case 1002:
                            baseMsg = new StartGameMsg();
                            baseMsg.Reading(receiveBytes, nowIndex);
                            break;
                        case 1003:
                            baseMsg = new QuitMsg();
                            baseMsg.Reading(receiveBytes, nowIndex);
                            break;
                        case 1004:
                            baseMsg = new PlayerInfoMsg();
                            baseMsg.Reading(receiveBytes, nowIndex);
                            break;
                        case 1005:
                            baseMsg = new DrawCardMsg();
                            baseMsg.Reading(receiveBytes, nowIndex);
                            break;
                        case 1006:
                            baseMsg = new ReplaceHPMsg();
                            baseMsg.Reading(receiveBytes, nowIndex);
                            break;
                        case 1007:
                            baseMsg = new ReplaceAtkMsg();
                            baseMsg.Reading(receiveBytes, nowIndex);
                            break;
                        case 1008:
                            baseMsg = new PlayerDefenceMsg();
                            baseMsg.Reading(receiveBytes, nowIndex);
                            break;
                        case 1009:
                            baseMsg = new EndMyTurnMsg();
                            baseMsg.Reading(receiveBytes, nowIndex);
                            break;
                        case 1010:
                            baseMsg = new AtkPlayerMsg();
                            baseMsg.Reading(receiveBytes, nowIndex);
                            break;
                        default:
                            Console.WriteLine("消息ID错误");
                            return;
                    }
                    if (baseMsg != null)
                    {
                        ThreadPool.QueueUserWorkItem(MsgHandle, baseMsg);
                    }
                    nowIndex += msgBodyLength;
                }
            }
        }
        private void MsgHandle(object obj)
        {
            BaseMsg msg = obj as BaseMsg;
            /*
            if (msg is ClientIdMsg)
            {
                ClientIdMsg clientIdMsg = msg as ClientIdMsg;
                Console.WriteLine("客户端ID：" + clientIdMsg.clientId);
            }
            */
            if (msg is QuitMsg)
            {
                // 客户端主动断开连接，删除字典中的该客户端
                Program.serverSocket.CloseClientSocket(this);
            }
            else if (msg is StartGameMsg)
            {
                StartGameMsg startGameMsg = msg as StartGameMsg;
                Program.serverSocket.ResetGame();
            }
            else if (msg is PlayerInfoMsg)
            {
                // 处理玩家信息消息
            }
            else if (msg is DrawCardMsg)
            {
                //处理抽牌消息
            }
            else if (msg is ReplaceHPMsg)
            {
                //处理替换生命值消息
                ReplaceHPMsg replaceHPMsg = msg as ReplaceHPMsg;
                Program.serverSocket.UpdatePlayerHPInfo(replaceHPMsg.clientId, replaceHPMsg.HP, replaceHPMsg.HpCardName);
            }
            else if (msg is ReplaceAtkMsg)
            {
                //处理替换攻击力消息
                ReplaceAtkMsg replaceAtkMsg = msg as ReplaceAtkMsg;
                Program.serverSocket.UpdatePlayerAtkInfo(replaceAtkMsg.clientId, replaceAtkMsg.atk, replaceAtkMsg.atkCardName);
            }
            else if (msg is PlayerDefenceMsg)
            {
                //处理防御消息
                PlayerDefenceMsg playerDefenceMsg = msg as PlayerDefenceMsg;
                Program.serverSocket.UpdatePlayerIsDefenceInfo(playerDefenceMsg.clientId, playerDefenceMsg.isDefence);
            }
            else if (msg is EndMyTurnMsg)
            {
                EndMyTurnMsg endMyTurnMsg = msg as EndMyTurnMsg;
                // 处理结束回合消息
                Program.serverSocket.NextPlayerTurn(endMyTurnMsg.clientId, endMyTurnMsg.nextClientId, endMyTurnMsg.isEndTurn);
            }
            else if (msg is AtkPlayerMsg)
            {
                AtkPlayerMsg atkPlayerMsg = msg as AtkPlayerMsg;
                // 处理攻击消息
                Program.serverSocket.PlayerAttack(atkPlayerMsg.clientId, atkPlayerMsg.targetClientId);
            }
        }
    }
}