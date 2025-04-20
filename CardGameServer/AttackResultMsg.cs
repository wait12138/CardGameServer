using System.Collections;
using System.Collections.Generic;

public class AttackResultMsg : BaseMsg
{
    public int clientId; // 对方玩家的ID
    public int value; // 显示的值
    public bool isHp; // 是否是血量 否则是攻击力
    public override byte[] Writing()
    {
        int index = 0;
        byte[] bytes = new byte[GetBytesNum()];
        WriteInt(bytes, GetID(), ref index); // 先写入消息ID
        WriteInt(bytes, GetBytesNum() - 8, ref index); // 再写入消息体长度
        WriteInt(bytes, clientId, ref index); // 再写入客户端ID
        WriteInt(bytes, value, ref index); // 再写入值
        WriteBool(bytes, isHp, ref index); // 再写入是否是血量
        return bytes;
    }
    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        clientId = ReadInt(bytes, ref index); // 读取客户端ID
        value = ReadInt(bytes, ref index); // 读取值
        isHp = ReadBool(bytes, ref index); // 读取是否是血量
        return index - beginIndex; // 返回读取的字节数
    }
    public override int GetBytesNum()
    {
        return 4 + // 消息ID
               4 + // 消息体长度
               4 + // 客户端ID
               4 + // 值
               1; // 是否是血量
    }
    public override int GetID()
    {
        return 1011; // 自定义攻击结果消息的ID为1011
    }
}
