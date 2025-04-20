using System.Collections;
using System.Collections.Generic;

public class EndMyTurnMsg : BaseMsg
{
    public int clientId; // 发送者的客户端ID
    public int nextClientId; // 下一个玩家的客户端ID
    public bool isEndTurn; // 是否结束回合
    public override byte[] Writing()
    {
        int index = 0;
        byte[] bytes = new byte[GetBytesNum()];
        WriteInt(bytes, GetID(), ref index); // 先写入消息ID
        WriteInt(bytes, GetBytesNum() - 8, ref index); // 再写入消息体长度
        WriteInt(bytes, clientId, ref index); // 再写入客户端ID
        WriteInt(bytes, nextClientId, ref index); // 再写入下一个玩家的客户端ID
        WriteBool(bytes, isEndTurn, ref index); // 再写入是否结束回合
        return bytes;
    }
    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        clientId = ReadInt(bytes, ref index); // 读取客户端ID
        nextClientId = ReadInt(bytes, ref index); // 读取下一个玩家的客户端ID
        isEndTurn = ReadBool(bytes, ref index); // 读取是否结束回合
        return index - beginIndex; // 返回读取的字节数
    }
    public override int GetBytesNum()
    {
        return sizeof(int) + // 消息ID
               sizeof(int) + // 消息体长度
               sizeof(int) + // 客户端ID
               sizeof(int) + // 下一个玩家的客户端ID
               sizeof(bool); // 是否结束回合
    }
    public override int GetID()
    {
        return 1009; // 自定义结束回合消息的ID为1009
    }
}
