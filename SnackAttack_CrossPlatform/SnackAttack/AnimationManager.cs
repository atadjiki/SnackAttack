using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class AnimationManager
{
    private AnimationManager animation;
    private float timer;

    public Vector2 Position { get; set; }

    public AnimationManager(Animation animation)
    {
        animation = animation;
    }
    public void Draw(SpriteBatch spritebatch)
    {
        spritebatch.Draw(animation.Texture, Position, new Rectangle(animation.CurrentFrame * animation.FrameWidth,
            0,
            animation.FrameWidth,
            animation.FrameHeight),
            Color.White);
    }

    public void Play(Animation animation)
    {
        if (animation == animation)
            return;

        animation = animation;
        animation.CurrentFrame = 0;
        timer = 0;

    }
    public void Stop()
    {
        timer = 0f;
        animation.CurrentFrame = 0;

    }

    public void Update(GameTime gameTime)
    {
        timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if(timer > animation.FrameSpeed)
        {
            timer = 0f;
            animation.CurrentFrame++;

            if (animation.CurrentFrame >= animation.FrameCount)
                animation.CurrentFrame = 0;
        }
    }
}
