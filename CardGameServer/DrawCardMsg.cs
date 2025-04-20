using System.Collections;
using System.Collections.Generic;
using System.Text;

public class DrawCardMsg : BaseMsg
{
    public int clientId; // 发送者的客户端ID
    public int cardValue; // 牌的值
    public string cardPrefabName; // 牌的预设体名称
    public override byte[] Writing()
    {
        int index = 0;
        byte[] bytes = new byte[GetBytesNum()];
        // 先写入消息ID
        WriteInt(bytes, GetID(), ref index);
        // 再写入消息体长度
        WriteInt(bytes, GetBytesNum() - 8, ref index); // 消息体长度 = 总长度 - 消息ID长度 - 消息体长度
        // 再写入客户端ID
        WriteInt(bytes, clientId, ref index);
        // 再写入牌的值
        WriteInt(bytes, cardValue, ref index);
        // 再写入牌的预设体名称
        WriteString(bytes, cardPrefabName, ref index);
        return bytes;
    }
    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        clientId = ReadInt(bytes, ref index); // 读取客户端ID
        cardValue = ReadInt(bytes, ref index); // 读取牌的值
        cardPrefabName = ReadString(bytes, ref index); // 读取牌的预设体名称
        return index - beginIndex; // 返回读取的字节数
    }
    public override int GetBytesNum()
    {
        return sizeof(int) + // 消息ID
               sizeof(int) + // 消息体长度
               sizeof(int) + // 客户端ID
               sizeof(int) + // 牌的值
               Encoding.UTF8.GetBytes(cardPrefabName).Length + 4; // 牌的预设体名称长度
    }
    public override int GetID()
    {
        return 1005; // 自定义抽牌消息的ID为1005
    }
}
