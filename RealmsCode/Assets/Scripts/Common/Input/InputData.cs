using System.Collections;
using System.Collections.Generic;
using MessagePack;
using Unity.Entities;

[MessagePackObject]
public struct InputData : IComponentData
{
    [Key(0)]
    public long Index;

    [Key(1)]
    public bool Right;

    [Key(2)]
    public bool Left;

    [Key(3)]
    public bool Up;

    [Key(4)]
    public bool Down;


    public override string ToString()
    {
        return $"{Right} {Left} {Up} {Down}";
    }

    public InputData(InputData data)
    {
        Index = data.Index;
        Right = data.Right;
        Left = data.Left;
        Up = data.Up;
        Down = data.Down;
    }
}