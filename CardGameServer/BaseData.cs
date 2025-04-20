using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public abstract class BaseData
{
    //用于子类重写的 获取字节数组容器大小的方法
    public abstract int GetBytesNum();
    //把成员变量 序列化为 对应的字节数组
    public abstract byte[] Writing();
    //把2进制字节数组 反序列化为 对应的成员变量
    public abstract int Reading(byte[] bytes, int beginIndex = 0);
    //存储int类型变量到指定的字节数组中
    //bytes:指定字节数组，value:要存储的int值，index:字节数组的索引
    protected void WriteInt(byte[] bytes, int value, ref int index)
    {
        BitConverter.GetBytes(value).CopyTo(bytes, index);
        index += sizeof(int);
    }
    //存储short类型变量到指定的字节数组中
    //bytes:指定字节数组，value:要存储的short值，index:字节数组的索引
    protected void WriteShort(byte[] bytes, short value, ref int index)
    {
        BitConverter.GetBytes(value).CopyTo(bytes, index);
        index += sizeof(short);
    }
    //存储long类型变量到指定的字节数组中
    //bytes:指定字节数组，value:要存储的long值，index:字节数组的索引
    protected void WriteLong(byte[] bytes, long value, ref int index)
    {
        BitConverter.GetBytes(value).CopyTo(bytes, index);
        index += sizeof(long);
    }
    //存储long类型变量到指定的字节数组中
    //bytes:指定字节数组，value:要存储的long值，index:字节数组的索引
    protected void WriteFloat(byte[] bytes, float value, ref int index)
    {
        BitConverter.GetBytes(value).CopyTo(bytes, index);
        index += sizeof(float);
    }
    protected void WriteByte(byte[] bytes, byte value, ref int index)
    {
        bytes[index] = value;
        index += sizeof(byte);
    }
    //存储bool类型变量到指定的字节数组中
    //bytes:指定字节数组，value:要存储的long值，index:字节数组的索引
    protected void WriteBool(byte[] bytes, bool value, ref int index)
    {
        BitConverter.GetBytes(value).CopyTo(bytes, index);
        index += sizeof(bool);
    }
    //存储string类型变量到指定的字节数组中
    //bytes:指定字节数组，value:要存储的string值，index:字节数组的索引
    protected void WriteString(byte[] bytes, string value, ref int index)
    {
        //先存储string字节数组的长度
        byte[] strBytes = Encoding.UTF8.GetBytes(value);
        WriteInt(bytes, strBytes.Length, ref index);
        //再存string字节数组
        strBytes.CopyTo(bytes, index);
        index += strBytes.Length;
    }
    protected void WriteData(byte[] bytes, BaseData data, ref int index)
    {
        data.Writing().CopyTo(bytes, index);
        index += data.GetBytesNum();
    }
    //读取int类型变量到指定的字节数组中
    //bytes:指定字节数组，index:字节数组的索引
    protected int ReadInt(byte[] bytes, ref int index)
    {
        int value = BitConverter.ToInt32(bytes, index);
        index += sizeof(int);
        return value;
    }
    //读取short类型变量到指定的字节数组中
    //bytes:指定字节数组，index:字节数组的索引
    protected short ReadShort(byte[] bytes, ref int index)
    {
        short value = BitConverter.ToInt16(bytes, index);
        index += sizeof(short);
        return value;
    }
    //读取long类型变量到指定的字节数组中
    //bytes:指定字节数组，index:字节数组的索引
    protected long ReadLong(byte[] bytes, ref int index)
    {
        long value = BitConverter.ToInt64(bytes, index);
        index += sizeof(long);
        return value;
    }
    //读取float类型变量到指定的字节数组中
    //bytes:指定字节数组，index:字节数组的索引
    protected float ReadFloat(byte[] bytes, ref int index)
    {
        float value = BitConverter.ToSingle(bytes, index);
        index += sizeof(float);
        return value;
    }
    protected byte ReadByte(byte[] bytes, ref int index)
    {
        byte value = bytes[index];
        index += sizeof(byte);
        return value;
    }
    //读取bool类型变量到指定的字节数组中
    //bytes:指定字节数组，index:字节数组的索引
    protected bool ReadBool(byte[] bytes, ref int index)
    {
        bool value = BitConverter.ToBoolean(bytes, index);
        index += sizeof(bool);
        return value;
    }
    //读取string类型变量到指定的字节数组中
    //bytes:指定字节数组，index:字节数组的索引
    protected string ReadString(byte[] bytes, ref int index)
    {
        //先读取长度
        int length = ReadInt(bytes, ref index);
        //在读取string字节数组
        string value = Encoding.UTF8.GetString(bytes, index, length);
        index += length;
        return value;
    }
    //读取BaseData类型变量到指定的字节数组中
    //bytes:指定字节数组，index:字节数组的索引
    protected T ReadData<T>(byte[] bytes, ref int index) where T : BaseData, new()
    {
        T value = new T();
        index += value.Reading(bytes, index);
        return value;
    }
}
