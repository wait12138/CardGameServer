using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
public class PlayerInfo : BaseData
{
    public int clientId;
    public string playerName;
    public int HP;
    public int Atk;
    public bool isDefence;
    public bool isDead;
    public bool isMyTurn;
    public string HpCardName; // 生命值卡牌名称
    public string AtkCardName; // 攻击力卡牌名称
    public override byte[] Writing()
    {
        int index = 0;
        byte[] bytes = new byte[GetBytesNum()];
        WriteInt(bytes, clientId, ref index);
        WriteString(bytes, playerName, ref index);
        WriteInt(bytes, HP, ref index);
        WriteInt(bytes, Atk, ref index);
        WriteBool(bytes, isDefence, ref index);
        WriteBool(bytes, isDead, ref index);
        WriteBool(bytes, isMyTurn, ref index);
        WriteString(bytes, HpCardName, ref index); // 写入生命值卡牌名称
        WriteString(bytes, AtkCardName, ref index); // 写入攻击力卡牌名称
        return bytes;
    }
    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        clientId = ReadInt(bytes, ref index);
        playerName = ReadString(bytes, ref index);
        HP = ReadInt(bytes, ref index);
        Atk = ReadInt(bytes, ref index);
        isDefence = ReadBool(bytes, ref index);
        isDead = ReadBool(bytes, ref index);
        isMyTurn = ReadBool(bytes, ref index);
        HpCardName = ReadString(bytes, ref index); // 读取生命值卡牌名称
        AtkCardName = ReadString(bytes, ref index); // 读取攻击力卡牌名称
        return index - beginIndex;
    }
    public override int GetBytesNum()
    {
        return 4 + // clientId
               4 + Encoding.UTF8.GetBytes(playerName).Length + // playerName
               4 + // HP
               4 + // Atk
               1 + // isDefence
               1 + // isDead
               1 + // isMyTurn
               4 + Encoding.UTF8.GetBytes(HpCardName).Length + // 生命值卡牌名称长度
               4 + Encoding.UTF8.GetBytes(AtkCardName).Length; // 攻击力卡牌名称长度
    }
}
