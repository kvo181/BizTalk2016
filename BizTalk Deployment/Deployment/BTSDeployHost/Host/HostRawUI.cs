using System;
using System.Management.Automation.Host;

using bizilante.Deployment.BTSDeployHost.Utils;

namespace bizilante.Deployment.BTSDeployHost.Host
{
    public class HostRawUI : PSHostRawUserInterface
    {
        private System.Windows.Forms.Control _control;
        public HostRawUI(System.Windows.Forms.Control control)
        {
            _control = control;
        }

        public override ConsoleColor ForegroundColor
        {
            get { return _control.BackColor.ToConsoleColor(); }
            set { }
        }

        public override ConsoleColor BackgroundColor
        {
            get { return _control.ForeColor.ToConsoleColor(); }
            set {  }
        }

        public override Coordinates CursorPosition
        {
            get { return new Coordinates(); }
            set { }
        }

        public override Coordinates WindowPosition
        {
            get { return new Coordinates(); }
            set { }
        }

        public override int CursorSize
        {
            get { return 0; }
            set { }
        }

        public override Size BufferSize
        {
            get
            {
                //var s = _control.ConsoleSizeInCharacters();
                //return new Size(s.Width, 9999);
                return new Size(256, 9999);
            }
            set { }
        }

        public override Size WindowSize
        {
            get
            {
                var s = _control.ConsoleSizeInCharacters();
                return new Size(s.Width, s.Height);
            }
            set { }
        }

        public override Size MaxWindowSize
        {
            get { return WindowSize; }
        }

        public override Size MaxPhysicalWindowSize
        {
            get { return MaxWindowSize; }
        }

        public override bool KeyAvailable
        {
            get { return false; }
        }

        public override string WindowTitle
        {
            get { return string.Empty; }
            set { }
        }

        public override KeyInfo ReadKey(ReadKeyOptions options)
        {
            throw new NotImplementedException();
        }

        public override void FlushInputBuffer()
        {
            throw new NotImplementedException();
        }

        public override void SetBufferContents(Coordinates origin, BufferCell[,] contents)
        {
        }

        public override void SetBufferContents(Rectangle rectangle, BufferCell fill)
        {
        }

        public override BufferCell[,] GetBufferContents(Rectangle rectangle)
        {
            return null;
        }

        public override void ScrollBufferContents(Rectangle source, Coordinates destination, Rectangle clip,
                                                  BufferCell fill)
        {
        }
    }
}