using bizilante.Deployment.BTSDeployHost.Console;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace bizilante.Deployment.BTSDeployHost.Utils
{
    public static class Extentions
    {
        public static ConsoleSize ToConsoleSize(this Size size)
        {
            return new ConsoleSize { Height = size.Height, Width = size.Width };
        }

        public static ConsoleSize ConsoleSizeInCharacters(this Control ctrl)
        {
            Size size = ctrl.GetSizeInCharacters();
            return size.ToConsoleSize();
        }
        public static Size GetSizeInCharacters(this Control ctrl)
        {
            Size size = new Size();
            if (ctrl.IsDisposed)
            {
                return size;
            }

            if (ctrl.InvokeRequired)
            {
                MethodInvoker inv = () => size = ctrl.GetSizeInCharacters();
                ctrl.Invoke(inv);
                return size;
            }

            using (Graphics g = ctrl.CreateGraphics())
            {
                Size charSize1 = TextRenderer.MeasureText(g, "<W>", ctrl.Font);
                Size charSize2 = TextRenderer.MeasureText(g, "<>", ctrl.Font);
                Size charSize = charSize1 - charSize2;

                Size border = SystemInformation.Border3DSize;

                int scrollbar = SystemInformation.VerticalScrollBarWidth;

                size = new Size(
                    Convert.ToInt32(
                        Math.Floor(
                            (float)(ctrl.ClientSize.Width - border.Width - scrollbar) /
                            charSize.Width)
                        ),
                    Convert.ToInt32(
                        Math.Floor((float)(ctrl.ClientSize.Height - border.Height) /
                                   charSize1.Height)
                        )
                    );
                size.Width -= 1;
            }

            return size;
        }

        public static IntPtr GetSafeWindowHandle(this Control ctrl)
        {
            if (ctrl.IsDisposed)
            {
                return IntPtr.Zero;
            }
            if (ctrl.InvokeRequired)
            {
                IntPtr value = IntPtr.Zero;
                MethodInvoker mi = () => value = ctrl.GetSafeWindowHandle();
                ctrl.Invoke(mi);
                return value;
            }

            return ctrl.Handle;
        }

    }
}
