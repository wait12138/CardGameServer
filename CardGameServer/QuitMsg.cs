public class QuitMsg : BaseMsg
{
    public override byte[] Writing()
    {
        int index = 0;
        byte[] bytes = new byte[GetBytesNum()];
        // 写入消息ID
        WriteInt(bytes, GetID(), ref index);
        // 写入消息体的长度
        WriteInt(bytes, bytes.Length - 8, ref index);
        return bytes;
    }
    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        return 0;
    }
    public override int GetBytesNum()
    {
        return sizeof(int) + sizeof(int); // 消息ID + 消息体长度
    }
    public override int GetID()
    {
        return 1003; // 定义退出消息的ID为1003
    }
}
