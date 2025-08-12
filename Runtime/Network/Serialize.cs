using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Muco {
    public class Serialize {
        public struct Byte4 {
            public byte x;
            public byte y;
            public byte z;
            public byte w;

            public Byte4(float value) {
                unsafe {
                    this = *(Byte4*)&value;
                }
            }

            public float GetFloat() {
                unsafe {
                    var bts = this;
                    return *(float*)&bts;
                }
            }

            public void Append(List<byte> buffer) {
                buffer.Add(x);
                buffer.Add(y);
                buffer.Add(z);
                buffer.Add(w);
            }

            public static bool TryRead(out Byte4 bts, ref int cursor, byte[] buffer) {
                bts = new Byte4 { };
                if (cursor + 4 > buffer.Length)
                    return false;
                bts.x = buffer[cursor++];
                bts.y = buffer[cursor++];
                bts.z = buffer[cursor++];
                bts.w = buffer[cursor++];
                return true;
            }
        }

        public struct Byte8 {
            public byte x0;
            public byte x1;
            public byte x2;
            public byte x3;
            public byte x4;
            public byte x5;
            public byte x6;
            public byte x7;

            public Byte8(double value) {
                unsafe {
                    this = *(Byte8*)&value;
                }
            }

            public double GetF64() {
                unsafe {
                    var bts = this;
                    return *(double*)&bts;
                }
            }

            public void Append(List<byte> buffer) {
                buffer.Add(x0);
                buffer.Add(x1);
                buffer.Add(x2);
                buffer.Add(x3);
                buffer.Add(x4);
                buffer.Add(x5);
                buffer.Add(x6);
                buffer.Add(x7);
            }

            public static bool TryRead(out Byte8 bts, ref int cursor, byte[] buffer) {
                bts = new Byte8 { };
                if (cursor + 8 > buffer.Length)
                    return false;
                bts.x0 = buffer[cursor++];
                bts.x1 = buffer[cursor++];
                bts.x2 = buffer[cursor++];
                bts.x3 = buffer[cursor++];
                bts.x4 = buffer[cursor++];
                bts.x5 = buffer[cursor++];
                bts.x6 = buffer[cursor++];
                bts.x7 = buffer[cursor++];
                return true;
            }
        }

        public static void SerFloat(float value, List<byte> buffer) {
            new Byte4(value).Append(buffer);
        }

        public static bool DesFloat(out float outValue, ref int cursor, byte[] buffer) {
            outValue = 0;
            Byte4 bts;
            if (!Byte4.TryRead(out bts, ref cursor, buffer))
                return false;
            outValue = bts.GetFloat();
            return true;
        }

        public static void SerF64(double value, List<byte> buffer) {
            new Byte8(value).Append(buffer);
        }

        public static bool DesF64(out double outValue, ref int cursor, byte[] buffer) {
            outValue = 0;
            Byte8 bts;
            if (!Byte8.TryRead(out bts, ref cursor, buffer))
                return false;
            outValue = bts.GetF64();
            return true;
        }

        public static void SerU8(byte value, List<byte> buffer) {
            buffer.Add(value);
        }

        public static bool DesU8(out byte outValue, ref int cursor, byte[] buffer) {
            outValue = 0;
            if (cursor + 1 > buffer.Length)
                return false;

            outValue = buffer[cursor++];
            return true;
        }

        public static void SerU16(ushort value, List<byte> buffer) {
            buffer.Add((byte)((value & 0xFF) >> 0));
            buffer.Add((byte)((value & 0xFFFF) >> 8));
        }

        public static bool DesU16(out ushort outValue, ref int cursor, byte[] buffer) {
            outValue = 0;
            if (cursor + 2 > buffer.Length)
                return false;

            outValue +=          buffer[cursor++];
            outValue += (ushort)(buffer[cursor++] << 8);
            return true;
        }

        public static void SerU32(uint value, List<byte> buffer) {
            buffer.Add((byte)((value & 0xFF) >> 0));
            buffer.Add((byte)((value & 0xFFFF) >> 8));
            buffer.Add((byte)((value & 0xFFFFFF) >> 16));
            buffer.Add((byte)((value & 0xFFFFFFFF) >> 24));
        }

        public static bool DesU32(out uint outValue, ref int cursor, byte[] buffer) {
            outValue = 0;
            if (cursor + 4 > buffer.Length)
                return false;

            outValue += (uint)buffer[cursor++];
            outValue += (uint)buffer[cursor++] << 8;
            outValue += (uint)buffer[cursor++] << 16;
            outValue += (uint)buffer[cursor++] << 24;
            return true;
        }

        public static void SerI32(int value, List<byte> buffer) {
            buffer.Add((byte)((value & 0xFF) >> 0));
            buffer.Add((byte)((value & 0xFFFF) >> 8));
            buffer.Add((byte)((value & 0xFFFFFF) >> 16));
            buffer.Add((byte)((value & 0xFFFFFFFF) >> 24));
        }

        public static bool DesI32(out int outValue, ref int cursor, byte[] buffer) {
            outValue = 0;
            if (cursor + 4 > buffer.Length)
                return false;

            outValue = buffer[cursor++];
            outValue += buffer[cursor++] << 8;
            outValue += buffer[cursor++] << 16;
            outValue += buffer[cursor++] << 24;
            return true;
        }

        public static void SerU64(ulong value, List<byte> buffer) {
            buffer.Add((byte)((value & 0xFF) >> 0));
            buffer.Add((byte)((value & 0xFFFF) >> 8));
            buffer.Add((byte)((value & 0xFFFFFF) >> 16));
            buffer.Add((byte)((value & 0xFFFFFFFF) >> 24));

            buffer.Add((byte)((value & 0xFFFFFFFFFF) >> 32));
            buffer.Add((byte)((value & 0xFFFFFFFFFFFF) >> 40));
            buffer.Add((byte)((value & 0xFFFFFFFFFFFFFF) >> 48));
            buffer.Add((byte)((value & 0xFFFFFFFFFFFFFFFF) >> 56));
        }

        public static bool DesU64(out ulong outValue, ref int cursor, byte[] buffer) {
            outValue = 0;
            if (cursor + 8 > buffer.Length)
                return false;

            outValue += (ulong)buffer[cursor++];
            outValue += (ulong)buffer[cursor++] << 8;
            outValue += (ulong)buffer[cursor++] << 16;
            outValue += (ulong)buffer[cursor++] << 24;

            outValue += (ulong)buffer[cursor++] << 32;
            outValue += (ulong)buffer[cursor++] << 40;
            outValue += (ulong)buffer[cursor++] << 48;
            outValue += (ulong)buffer[cursor++] << 56;
            return true;
        }
        
        public static void SerBool(bool value, List<byte> buffer) {
            buffer.Add(value ? (byte)1 : (byte)0);
        }

        public static bool DesBool(out bool outValue, ref int cursor, byte[] buffer) {
            outValue = false;
            if (cursor >= buffer.Length)
                return false;
            var b = buffer[cursor++];
            if (b != 0)
                outValue = true;
            return true;
        }

        public static void SerByteArr(byte[] value, List<byte> buffer) {
            SerI32(value.Length, buffer);
            buffer.AddRange(value);
        }

        public static bool DesByteArr(out byte[] outValue, ref int cursor, byte[] buffer) {
            outValue = new byte[] { };
            int length;
            if (!DesI32(out length, ref cursor, buffer))
                return false;
            int end = cursor + length;
            if (end > buffer.Length)
                return false;
            outValue = new byte[length];

            for (int i = 0; i < length; i++)
                outValue[i] = buffer[cursor++];

            return true;
        }

        public static void SerString(string value, List<byte> buffer) {
            if (value == null)
                SerString("", buffer);
            else {
                SerByteArr(Encoding.ASCII.GetBytes(value), buffer);
            }
        }

        public static bool DesString(out string outValue, ref int cursor, byte[] buffer) {
            outValue = "";
            byte[] stringBytes;
            if (!DesByteArr(out stringBytes, ref cursor, buffer))
                return false;
            outValue = Encoding.UTF8.GetString(stringBytes);
            return true;
        }

        public static void SerVector3(Vector3 value, List<byte> buffer)
        {
            SerFloat(value.x, buffer);
            SerFloat(value.y, buffer);
            SerFloat(value.z, buffer);
        }

        public static bool DesVector3(out Vector3 outValue, ref int cursor, byte[] buffer)
        {
            outValue = Vector3.zero;
            if (!DesFloat(out outValue.x, ref cursor, buffer))
                return false;
            if (!DesFloat(out outValue.y, ref cursor, buffer))
                return false;
            if (!DesFloat(out outValue.z, ref cursor, buffer))
                return false;
            return true;
        }

        public static void SerQuat(Quaternion value, List<byte> buffer)
        {
            SerFloat(value.x, buffer);
            SerFloat(value.y, buffer);
            SerFloat(value.z, buffer);
            SerFloat(value.w, buffer);
        }

        public static bool DesQuat(out Quaternion outValue, ref int cursor, byte[] buffer)
        {
            outValue = Quaternion.identity;
            if (!DesFloat(out outValue.x, ref cursor, buffer))
                return false;
            if (!DesFloat(out outValue.y, ref cursor, buffer))
                return false;
            if (!DesFloat(out outValue.z, ref cursor, buffer))
                return false;
            if (!DesFloat(out outValue.w, ref cursor, buffer))
                return false;
            return true;
        }

        public static void SerColor(Color value, List<byte> buffer)
        {
            SerFloat(value.r, buffer);
            SerFloat(value.g, buffer);
            SerFloat(value.b, buffer);
            SerFloat(value.a, buffer);
        }

        public static bool DesColor(out Color outValue, ref int cursor, byte[] buffer)
        {
            outValue = Color.black;
            if (!DesFloat(out outValue.r, ref cursor, buffer))
                return false;
            if (!DesFloat(out outValue.g, ref cursor, buffer))
                return false;
            if (!DesFloat(out outValue.b, ref cursor, buffer))
                return false;
            if (!DesFloat(out outValue.a, ref cursor, buffer))
                return false;
            return true;
        }

        public static void SerInteractorId(InteractorId value, List<byte> buffer) {
            SerU16(value.user_id, buffer);
            SerU8(value.system_id, buffer);
            SerU8(value.interactor_id, buffer);
        }

        public static bool DesInteractorId(out InteractorId outValue, ref int cursor, byte[] buffer) {
            outValue = new InteractorId { };

            if (!DesU16(out outValue.user_id, ref cursor, buffer))
                return false;
            if (!DesU8(out outValue.system_id, ref cursor, buffer))
                return false;
            if (!DesU8(out outValue.interactor_id, ref cursor, buffer))
                return false;

            return true;
        }

        public static void SerTransLocal(Transform value, List<byte> buffer)
        {
            SerVector3(value.localPosition, buffer);
            SerQuat(value.localRotation, buffer);
        }

        public static bool DesTransLocal(Transform outValue, ref int cursor, byte[] buffer)
        {
            Vector3 pos;
            if (!DesVector3(out pos, ref cursor, buffer))
                return false;
            outValue.localPosition = pos;

            Quaternion rot;
            if (!DesQuat(out rot, ref cursor, buffer))
                return false;
            outValue.localRotation = rot;

            return true;
        }

        public static void SerTransGlobal(Transform value, List<byte> buffer)
        {
            SerVector3(value.position, buffer);
            SerQuat(value.rotation, buffer);
        }

        public static bool DesTransGlobal(Transform outValue, ref int cursor, byte[] buffer)
        {
            Vector3 pos;
            if (!DesVector3(out pos, ref cursor, buffer))
                return false;
            outValue.position = pos;

            Quaternion rot;
            if (!DesQuat(out rot, ref cursor, buffer))
                return false;
            outValue.rotation = rot;

            return true;
        }

        public static void SerTransArrLocal(List<Transform> value, List<byte> buffer)
        {
            SerI32(value.Count, buffer);
            foreach (var trans in value) {
                SerVector3(trans.localPosition, buffer);
            }
            foreach (var trans in value) {
                SerQuat(trans.localRotation, buffer);
            }
        }

        public static bool DesTransArrLocal(List<Transform> outValue, ref int cursor, byte[] buffer)
        {
            int length;
            if (!DesI32(out length, ref cursor, buffer))
                return false;

            if (length != outValue.Count) {
                Debug.Log("mismatch transform list length while deserializing: expected " + outValue.Count + " got " + length);
                return false;
            }

            for (int i = 0; i < length; i++)
            {
                Vector3 localPosition;
                if (!DesVector3(out localPosition, ref cursor, buffer))
                    return false;
                outValue[i].localPosition = localPosition;
            }
            for (int i = 0; i < length; i++)
            {
                Quaternion localRotation;
                if (!DesQuat(out localRotation, ref cursor, buffer))
                    return false;
                outValue[i].localRotation = localRotation;
            }

            return true;
        }

        public static void DiffData(List<byte> a, List<byte> b, List<byte> diff) {
            diff.Clear();
            var len = b.Count;
            EncodeVlq(len, diff);
            while (a.Count < len)
                a.Add(0);
            int cursor = 0;
            while (cursor < len) {
                var same = CountSame(a, b, ref cursor);
                EncodeVlq(same, diff);
                if (cursor >= len)
                    break;
                var end = cursor;
                var different = CountDifferent(a, b, ref end);
                EncodeVlq(different, diff);
                for (; cursor < end; cursor++)
                    diff.Add(b[cursor]);
            }
        }

        public static bool UpdateFromDiff(List<byte> a, byte[] diff, ref int cursor) {
            int bufferCursor = 0;
            int len;
            if (!DecodeVlq(ref cursor, diff, out len))
                return false;
            
            while (a.Count < len)
                a.Add(0);
            
            while (bufferCursor < len) {
                int same;
                if (!DecodeVlq(ref cursor, diff, out same))
                    return false;
                bufferCursor += same;
                
                if (bufferCursor == len)
                    break;
                
                int different;
                if (!DecodeVlq(ref cursor, diff, out different))
                    return false;
                for (int i = 0; i < different; i++) {
                    a[bufferCursor++] = diff[cursor++];
                }
            }

            return true;
        }

        public static int CountSame(List<byte> a, List<byte> b, ref int cursor) {
            var begin = cursor;
            while (cursor < b.Count) {
                if (a[cursor] != b[cursor])
                    break;
                cursor++;
            }
            return cursor - begin;
        }

        public static int CountDifferent(List<byte> a, List<byte> b, ref int cursor) {
            var begin = cursor;
            while (cursor < b.Count) {
                if (a[cursor] == b[cursor])
                    break;
                cursor++;
            }
            return cursor - begin;
        }

        public static void EncodeVlq(int x, List<byte> buffer) {
            while (true) {
                byte a = (byte)(x & 0b1111111);
                x = x >> 7;
                if (x == 0) {
                    buffer.Add(a);
                    return;
                }
                else {
                    buffer.Add((byte)(a | 0b10000000));
                }
            }
        }

        public static bool DecodeVlq(ref int cursor, byte[] buffer, out int result) {
            result = 0;
            var shift = 0;
            while(true) {
                if (cursor >= buffer.Length)
                    return false;
                var b = buffer[cursor];
                cursor += 1;
                result += (b & 0b1111111) << shift;
                if ((b & 0b10000000) == 0) 
                    break;
                shift += 7;
            }

            return true;
        }
    }
}
