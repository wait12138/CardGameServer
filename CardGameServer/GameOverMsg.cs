using System.Collections;
using System.Collections.Generic;

public class GameOverMsg : BaseMsg
{
    public int winnerId; // 胜利者ID
    public bool isGameOver; // 游戏是否结束
    public override byte[] Writing()
    {
        int index = 0;
        byte[] bytes = new byte[GetBytesNum()];
        WriteInt(bytes, GetID(), ref index); // 先写入消息ID
        WriteInt(bytes, GetBytesNum() - 8, ref index); // 再写入消息体长度
        WriteInt(bytes, winnerId, ref index); // 再写入胜利者ID
        WriteBool(bytes, isGameOver, ref index); // 再写入游戏是否结束
        return bytes;
    }
    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        winnerId = ReadInt(bytes, ref index); // 读取胜利者ID
        isGameOver = ReadBool(bytes, ref index); // 读取游戏是否结束
        return index - beginIndex; // 返回读取的字节数
    }
    public override int GetBytesNum()
    {
        return 4 + // 消息ID
               4 + // 消息体长度
               4 + // 胜利者ID
               1; // 游戏是否结束
    }
    public override int GetID()
    {
        return 1012; // 自定义游戏结束消息的ID为1012
    }
}
