using Microsoft.Toolkit.Uwp.Notifications;
using TomShowsIt.Editor;
using TomShowsIt.Screenshot;
using TomShowsIt.Utilities;

namespace TomShowsIt
{
    public class TrayApplicationContext : ApplicationContext
    {
        private readonly NotifyIcon _trayIcon;

        private bool _editorOpen = false;

        public TrayApplicationContext()
        {
            _trayIcon = new NotifyIcon
            {
                Icon = new Icon("icon.ico"),
                ContextMenuStrip = new ContextMenuStrip(),
                Visible = true
            };

            _trayIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Exit", null, ExitApplication));

            InitializeHook();
        }

        private void ExitApplication(object? sender, EventArgs e)
        {
            _trayIcon.Visible = false;
            var dialogResult = MessageBox.Show("Are you sure you want to close 'TomShowsIt'?", "Are you sure?", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void InitializeHook()
        {
            var hook = new KeyboardHook();

            hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(OnHotKeyPressed);

            hook.RegisterHotKey(ModifierKeys.Win | ModifierKeys.Shift, Keys.Tab);
        }

        private void OnHotKeyPressed(object? sender, KeyPressedEventArgs e)
        {
            if (_editorOpen) return;

            var img = SnippingTool.CaptureRegion();

            if (img != null)
            {
                var editor = new TomShowsItEditor(img);

                _editorOpen = true;
                if (editor.ShowDialog() == true)
                {
                    Clipboard.SetImage(editor.GetResult());

                    new ToastContentBuilder()
                        .AddText("Copied to clipboard!")
                        .Show(toast =>
                        {
                            toast.ExpirationTime = DateTime.Now.AddMinutes(5);
                        });
                }
                _editorOpen = false;
            }
        }
    }
}
