using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Animation
{
    public int CurrentFrame { get; set; }
    public int FrameCount { get; private set; }
    public int FrameHeight { get { return Texture.Height; } }
    public float FrameSpeed { get; get }
    public int FrameWidth { get; { return Texture.Width / FrameCount; } }
    public int IsLooping { get; set }
    public Texture2D Texture { get; private set;}

    public Animation(Texture2D texture, int frameCount)
    {
        Texture = texture;
        FrameCount = frameCount;
        IsLooping = true;
        FrameSpeed = 0.2f;

    }
}
