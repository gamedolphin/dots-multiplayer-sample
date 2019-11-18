#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168

namespace MessagePack.Resolvers
{
    using System;
    using MessagePack;

    public class GeneratedResolver : global::MessagePack.IFormatterResolver
    {
        public static readonly global::MessagePack.IFormatterResolver Instance = new GeneratedResolver();

        GeneratedResolver()
        {

        }

        public global::MessagePack.Formatters.IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly global::MessagePack.Formatters.IMessagePackFormatter<T> formatter;

            static FormatterCache()
            {
                var f = GeneratedResolverGetFormatterHelper.GetFormatter(typeof(T));
                if (f != null)
                {
                    formatter = (global::MessagePack.Formatters.IMessagePackFormatter<T>)f;
                }
            }
        }
    }

    internal static class GeneratedResolverGetFormatterHelper
    {
        static readonly global::System.Collections.Generic.Dictionary<Type, int> lookup;

        static GeneratedResolverGetFormatterHelper()
        {
            lookup = new global::System.Collections.Generic.Dictionary<Type, int>(5)
            {
                {typeof(global::System.Collections.Generic.List<global::PlayerState>), 0 },
                {typeof(global::InputData), 1 },
                {typeof(global::Vector3Sim), 2 },
                {typeof(global::PlayerState), 3 },
                {typeof(global::WorldState), 4 },
            };
        }

        internal static object GetFormatter(Type t)
        {
            int key;
            if (!lookup.TryGetValue(t, out key)) return null;

            switch (key)
            {
                case 0: return new global::MessagePack.Formatters.ListFormatter<global::PlayerState>();
                case 1: return new MessagePack.Formatters.InputDataFormatter();
                case 2: return new MessagePack.Formatters.Vector3SimFormatter();
                case 3: return new MessagePack.Formatters.PlayerStateFormatter();
                case 4: return new MessagePack.Formatters.WorldStateFormatter();
                default: return null;
            }
        }
    }
}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612



#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168

namespace MessagePack.Formatters
{
    using System;
    using MessagePack;


    public sealed class InputDataFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::InputData>
    {

        public int Serialize(ref byte[] bytes, int offset, global::InputData value, global::MessagePack.IFormatterResolver formatterResolver)
        {
            
            var startOffset = offset;
            offset += global::MessagePack.MessagePackBinary.WriteFixedArrayHeaderUnsafe(ref bytes, offset, 5);
            offset += MessagePackBinary.WriteInt64(ref bytes, offset, value.Index);
            offset += MessagePackBinary.WriteBoolean(ref bytes, offset, value.Right);
            offset += MessagePackBinary.WriteBoolean(ref bytes, offset, value.Left);
            offset += MessagePackBinary.WriteBoolean(ref bytes, offset, value.Up);
            offset += MessagePackBinary.WriteBoolean(ref bytes, offset, value.Down);
            return offset - startOffset;
        }

        public global::InputData Deserialize(byte[] bytes, int offset, global::MessagePack.IFormatterResolver formatterResolver, out int readSize)
        {
            if (global::MessagePack.MessagePackBinary.IsNil(bytes, offset))
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }

            var startOffset = offset;
            var length = global::MessagePack.MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
            offset += readSize;

            var __Index__ = default(long);
            var __Right__ = default(bool);
            var __Left__ = default(bool);
            var __Up__ = default(bool);
            var __Down__ = default(bool);

            for (int i = 0; i < length; i++)
            {
                var key = i;

                switch (key)
                {
                    case 0:
                        __Index__ = MessagePackBinary.ReadInt64(bytes, offset, out readSize);
                        break;
                    case 1:
                        __Right__ = MessagePackBinary.ReadBoolean(bytes, offset, out readSize);
                        break;
                    case 2:
                        __Left__ = MessagePackBinary.ReadBoolean(bytes, offset, out readSize);
                        break;
                    case 3:
                        __Up__ = MessagePackBinary.ReadBoolean(bytes, offset, out readSize);
                        break;
                    case 4:
                        __Down__ = MessagePackBinary.ReadBoolean(bytes, offset, out readSize);
                        break;
                    default:
                        readSize = global::MessagePack.MessagePackBinary.ReadNextBlock(bytes, offset);
                        break;
                }
                offset += readSize;
            }

            readSize = offset - startOffset;

            var ____result = new global::InputData();
            ____result.Index = __Index__;
            ____result.Right = __Right__;
            ____result.Left = __Left__;
            ____result.Up = __Up__;
            ____result.Down = __Down__;
            return ____result;
        }
    }


    public sealed class Vector3SimFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Vector3Sim>
    {

        public int Serialize(ref byte[] bytes, int offset, global::Vector3Sim value, global::MessagePack.IFormatterResolver formatterResolver)
        {
            
            var startOffset = offset;
            offset += global::MessagePack.MessagePackBinary.WriteFixedArrayHeaderUnsafe(ref bytes, offset, 3);
            offset += MessagePackBinary.WriteSingle(ref bytes, offset, value.x);
            offset += MessagePackBinary.WriteSingle(ref bytes, offset, value.y);
            offset += MessagePackBinary.WriteSingle(ref bytes, offset, value.z);
            return offset - startOffset;
        }

        public global::Vector3Sim Deserialize(byte[] bytes, int offset, global::MessagePack.IFormatterResolver formatterResolver, out int readSize)
        {
            if (global::MessagePack.MessagePackBinary.IsNil(bytes, offset))
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }

            var startOffset = offset;
            var length = global::MessagePack.MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
            offset += readSize;

            var __x__ = default(float);
            var __y__ = default(float);
            var __z__ = default(float);

            for (int i = 0; i < length; i++)
            {
                var key = i;

                switch (key)
                {
                    case 0:
                        __x__ = MessagePackBinary.ReadSingle(bytes, offset, out readSize);
                        break;
                    case 1:
                        __y__ = MessagePackBinary.ReadSingle(bytes, offset, out readSize);
                        break;
                    case 2:
                        __z__ = MessagePackBinary.ReadSingle(bytes, offset, out readSize);
                        break;
                    default:
                        readSize = global::MessagePack.MessagePackBinary.ReadNextBlock(bytes, offset);
                        break;
                }
                offset += readSize;
            }

            readSize = offset - startOffset;

            var ____result = new global::Vector3Sim();
            ____result.x = __x__;
            ____result.y = __y__;
            ____result.z = __z__;
            return ____result;
        }
    }


    public sealed class PlayerStateFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::PlayerState>
    {

        public int Serialize(ref byte[] bytes, int offset, global::PlayerState value, global::MessagePack.IFormatterResolver formatterResolver)
        {
            
            var startOffset = offset;
            offset += global::MessagePack.MessagePackBinary.WriteFixedArrayHeaderUnsafe(ref bytes, offset, 3);
            offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.Id);
            offset += formatterResolver.GetFormatterWithVerify<global::Vector3Sim>().Serialize(ref bytes, offset, value.Position, formatterResolver);
            offset += MessagePackBinary.WriteInt64(ref bytes, offset, value.Index);
            return offset - startOffset;
        }

        public global::PlayerState Deserialize(byte[] bytes, int offset, global::MessagePack.IFormatterResolver formatterResolver, out int readSize)
        {
            if (global::MessagePack.MessagePackBinary.IsNil(bytes, offset))
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }

            var startOffset = offset;
            var length = global::MessagePack.MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
            offset += readSize;

            var __Id__ = default(int);
            var __Position__ = default(global::Vector3Sim);
            var __Index__ = default(long);

            for (int i = 0; i < length; i++)
            {
                var key = i;

                switch (key)
                {
                    case 0:
                        __Id__ = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
                        break;
                    case 1:
                        __Position__ = formatterResolver.GetFormatterWithVerify<global::Vector3Sim>().Deserialize(bytes, offset, formatterResolver, out readSize);
                        break;
                    case 2:
                        __Index__ = MessagePackBinary.ReadInt64(bytes, offset, out readSize);
                        break;
                    default:
                        readSize = global::MessagePack.MessagePackBinary.ReadNextBlock(bytes, offset);
                        break;
                }
                offset += readSize;
            }

            readSize = offset - startOffset;

            var ____result = new global::PlayerState();
            ____result.Id = __Id__;
            ____result.Position = __Position__;
            ____result.Index = __Index__;
            return ____result;
        }
    }


    public sealed class WorldStateFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::WorldState>
    {

        public int Serialize(ref byte[] bytes, int offset, global::WorldState value, global::MessagePack.IFormatterResolver formatterResolver)
        {
            
            var startOffset = offset;
            offset += global::MessagePack.MessagePackBinary.WriteFixedArrayHeaderUnsafe(ref bytes, offset, 1);
            offset += formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::PlayerState>>().Serialize(ref bytes, offset, value.playerState, formatterResolver);
            return offset - startOffset;
        }

        public global::WorldState Deserialize(byte[] bytes, int offset, global::MessagePack.IFormatterResolver formatterResolver, out int readSize)
        {
            if (global::MessagePack.MessagePackBinary.IsNil(bytes, offset))
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }

            var startOffset = offset;
            var length = global::MessagePack.MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
            offset += readSize;

            var __playerState__ = default(global::System.Collections.Generic.List<global::PlayerState>);

            for (int i = 0; i < length; i++)
            {
                var key = i;

                switch (key)
                {
                    case 0:
                        __playerState__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::PlayerState>>().Deserialize(bytes, offset, formatterResolver, out readSize);
                        break;
                    default:
                        readSize = global::MessagePack.MessagePackBinary.ReadNextBlock(bytes, offset);
                        break;
                }
                offset += readSize;
            }

            readSize = offset - startOffset;

            var ____result = new global::WorldState();
            ____result.playerState = __playerState__;
            return ____result;
        }
    }

}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612
