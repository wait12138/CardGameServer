using System.Collections;
using System.Collections.Generic;
using System.Text;

public class ReplaceHPMsg : BaseMsg
{
    public int clientId; // 发送者的客户端ID
    public int HP; // 生命值
    public string HpCardName; // 生命值卡牌名称
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
        // 再写入生命值
        WriteInt(bytes, HP, ref index);
        // 再写入生命值卡牌名称
        WriteString(bytes, HpCardName, ref index);
        return bytes;
    }

    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        clientId = ReadInt(bytes, ref index); // 读取客户端ID
        HP = ReadInt(bytes, ref index); // 读取生命值
        HpCardName = ReadString(bytes, ref index); // 读取生命值卡牌名称
        return index - beginIndex; // 返回读取的字节数
    }

    public override int GetBytesNum()
    {
        return sizeof(int) + // 消息ID
               sizeof(int) + // 消息体长度
               sizeof(int) + // 客户端ID
               sizeof(int) + // 生命值
                4 + Encoding.UTF8.GetBytes(HpCardName).Length; // 生命值卡牌名称长度
    }

    public override int GetID()
    {
        return 1006; // 自定义替换生命值消息的ID为1006
    }
}
