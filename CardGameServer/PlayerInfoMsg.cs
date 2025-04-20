using System.Text;

namespace CardGameServer
{
    class PlayerInfoMsg : BaseMsg
    {
        public bool isInitRound; // 是否是初始化回合
        public bool isGameRound; // 是否是游戏回合
        public PlayerInfo playerInfo1;
        public PlayerInfo playerInfo2; // 玩家信息
        public PlayerInfo playerInfo3; // 玩家信息
        public PlayerInfo playerInfo4; // 玩家信息
        public override byte[] Writing()
        {
            int index = 0;
            int bytesNum = GetBytesNum();
            byte[] bytes = new byte[bytesNum];
            //先写消息ID
            WriteInt(bytes, GetID(), ref index);
            //写如消息体的长度 我们-8的目的 是只存储 消息体的长度 前面8个字节 是我们自己定的规则 解析时按照这个规则处理就行了
            WriteInt(bytes, bytesNum - 8, ref index);
            //写这个消息的成员变量
            WriteBool(bytes, isInitRound, ref index);
            WriteBool(bytes, isGameRound, ref index);
            WriteData(bytes, playerInfo1, ref index);
            WriteData(bytes, playerInfo2, ref index);
            WriteData(bytes, playerInfo3, ref index);
            WriteData(bytes, playerInfo4, ref index);
            return bytes;
        }
        public override int Reading(byte[] bytes, int beginIndex = 0)
        {
            int index = beginIndex;
            isInitRound = ReadBool(bytes, ref index);
            isGameRound = ReadBool(bytes, ref index);
            playerInfo1 = ReadData<PlayerInfo>(bytes, ref index);
            playerInfo2 = ReadData<PlayerInfo>(bytes, ref index);
            playerInfo3 = ReadData<PlayerInfo>(bytes, ref index);
            playerInfo4 = ReadData<PlayerInfo>(bytes, ref index);
            return index - beginIndex;
        }
        public override int GetBytesNum()
        {
            return 4 + //消息ID的长度
                 4 + //消息体的长度
                 1 + //isInitRound的长度
                 1 + //isGameRound的长度
                 playerInfo1.GetBytesNum() + //playerInfo1的字节数组长度
                 playerInfo2.GetBytesNum() +   //playerInfo2的字节数组长度
                 playerInfo3.GetBytesNum() + //playerInfo3的字节数组长度
                 playerInfo4.GetBytesNum();   //playerInfo4的字节数组长度
        }
        public override int GetID()
        {
            return 1004;//自定义玩家信息消息的ID为1004
        }
    }
}