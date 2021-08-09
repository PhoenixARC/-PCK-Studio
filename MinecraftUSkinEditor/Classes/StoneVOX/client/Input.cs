using OpenTK;
using OpenTK.Input;
using System.Collections.Generic;

namespace stonevox
{
    public delegate void KeyHandler(KeyboardKeyEventArgs e);
    public delegate void KeyPressHandler(KeyPressEventArgs e);
    public delegate void MouseHandler(MouseButtonEventArgs e);
    public delegate void MouseMoveHandler(MouseMoveEventArgs e);
    public delegate void MouseWheelHandler(MouseWheelEventArgs e);

    public class Input : Singleton<Input>
    {

        public List<InputHandler> messaginghandlers = new List<InputHandler>();
        public List<InputHandler> driverhandlers = new List<InputHandler>();

        public int mousedx;
        public int mousedy;

        private KeyboardState lastKeyboardstate;
        private KeyboardState currentKeyboardstate;

        private MouseState lastmousestate;
        private MouseState currentmousestate;

        public int mousex;
        public int mousey;

        public int mouseX { get { return currentmousestate.X; } }
        public int mouseY { get { return currentmousestate.Y; } }

        private GLWindow window;

        private bool isFocused { get { return window.isfocused; } }

        public Input(GLWindow window)
             : base()
        {
            this.window = window;
        }

        public void update()
        {
            currentKeyboardstate = Keyboard.GetState();
            currentmousestate = Mouse.GetState();

            mousedx = currentmousestate.X - lastmousestate.X;
            mousedy = currentmousestate.Y - lastmousestate.Y;

            lastKeyboardstate = currentKeyboardstate;
            lastmousestate = currentmousestate;
        }

        public void AddHandler(InputHandler handler)
        {
            messaginghandlers.Add(handler);
        }

        public void removehandler(InputHandler handler)
        {
            messaginghandlers.Remove(handler);
        }

        public bool Keydown(Key Key)
        {
            return isFocused && currentKeyboardstate.IsKeyDown(Key);
        }
        public bool Keyup(Key Key)
        {
            return isFocused && currentKeyboardstate.IsKeyUp(Key);
        }
        public bool Keypressed(Key Key)
        {
            return isFocused && currentKeyboardstate.IsKeyDown(Key) && lastKeyboardstate.IsKeyUp(Key);
        }

        public bool mousedown(MouseButton button)
        {
            return isFocused && currentmousestate.IsButtonDown(button);
        }
        public bool mouseup(MouseButton button)
        {
            return isFocused && currentmousestate.IsButtonUp(button);
        }
        public bool mousepressed(MouseButton button)
        {
            return isFocused && currentmousestate.IsButtonDown(button) && lastmousestate.IsButtonUp(button);
        }

        public void handleKeydown(KeyboardKeyEventArgs e)
        {
            if (!isFocused) return;
            messaginghandlers.ForEach(delegate (InputHandler t)
            {
                if (t.Keydownhandler != null)
                    t.Keydownhandler(e);
            });
        }

        public void handleKeyup(KeyboardKeyEventArgs e)
        {
            //Client.print("info", "Keyup");
            if (!isFocused) return;

            messaginghandlers.ForEach(delegate (InputHandler t)
            {
                if (t.Keyuphandler != null)
                    t.Keyuphandler(e);
            });
        }

        public void handleKeypress(KeyPressEventArgs e)
        {
            ///Client.print("info", "Keypress");
            if (!isFocused) return;
            messaginghandlers.ForEach(delegate (InputHandler t)
            {
                if (t.Keypresshandler != null)
                    t.Keypresshandler(e);
            });
        }

        public void handlemousemove(MouseMoveEventArgs e)
        {
            if (!isFocused) return;

            messaginghandlers.ForEach(delegate (InputHandler t)
            {
                if (t.mousemovehandler != null)
                    t.mousemovehandler(e);
            });
        }

        public void handlemousedown(MouseButtonEventArgs e)
        {
            //Client.print("info", "mousedown");
            if (!isFocused) return;

            messaginghandlers.ForEach(delegate (InputHandler t)
            {
                if (t.mousedownhandler != null)
                    t.mousedownhandler(e);
            });
        }

        public void handlemouseup(MouseButtonEventArgs e)
        {
            ///Client.print("info", "mouseup");
            if (!isFocused) return;
            messaginghandlers.ForEach(delegate (InputHandler t)
            {
                if (t.mouseuphandler != null)
                    t.mouseuphandler(e);
            });
        }

        public void handlemousewheel(MouseWheelEventArgs e)
        {
            // Client.print("info", "mousewheel");
            if (!isFocused) return;

            messaginghandlers.ForEach(delegate (InputHandler t)
            {
                if (t.mousewheelhandler != null)
                    t.mousewheelhandler(e);
            });
        }
    }

    public class InputHandler
    {
        public KeyHandler Keydownhandler;
        public KeyHandler Keyuphandler;
        public KeyPressHandler Keypresshandler;
        public MouseHandler mousedownhandler;
        public MouseHandler mouseuphandler;
        public MouseMoveHandler mousemovehandler;
        public MouseWheelHandler mousewheelhandler;
    }
}