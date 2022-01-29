using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;

namespace Server_GUI2
{
    public partial class ProgressForm : Window, IDisposable
    {
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private bool disposedValue;
        // private System.ComponentModel.Container components = null;

        public ProgressForm()
        {
            //
            // Windows フォーム デザイナ サポートに必要です。
            //
            InitializeComponent();

            //
            // TODO: InitializeComponent 呼び出しの後に、コンストラクタ コードを追加してください。
            //
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージド状態を破棄します (マネージド オブジェクト)
                }

                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
                // TODO: 大きなフィールドを null に設定します
                disposedValue = true;
            }
        }

        // // TODO: 'Dispose(bool disposing)' にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします
        // ~ProgressBar()
        // {
        //     // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        // /// <summary>
        // /// 使用されているリソースに後処理を実行します。
        // /// </summary>
        // protected void Dispose(bool disposing)
        // {
        //     if (disposing)
        //     {
        //         if (components != null)
        //         {
        //             components.Dispose();
        //         }
        //     }
        //     base.Dispose(disposing);
        // }

        // #region Windows フォーム デザイナで生成されたコード 
        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        // private void InitializeComponent()
        // {
        //     // this.Label1 = new System.Windows.Forms.Label();
        //     // this.ProgressBar1 = new System.Windows.Forms.ProgressBar();
        //     // this.Button1 = new System.Windows.Forms.Button();
        //     // this.SuspendLayout();
        //     // // 
        //     // // Label1
        //     // // 
        //     // this.Label1.Location = new System.Drawing.Point(16, 8);
        //     // this.Label1.Name = "Label1";
        //     // this.Label1.Size = new System.Drawing.Size(288, 40);
        //     // this.Label1.TabIndex = 0;
        //     // // 
        //     // // ProgressBar1
        //     // // 
        //     // this.ProgressBar1.Location = new System.Drawing.Point(8, 48);
        //     // this.ProgressBar1.Name = "ProgressBar1";
        //     // this.ProgressBar1.Size = new System.Drawing.Size(216, 23);
        //     // this.ProgressBar1.TabIndex = 1;
        //     // // 
        //     // // Button1
        //     // // 
        //     // this.Button1.Location = new System.Drawing.Point(232, 48);
        //     // this.Button1.Name = "Button1";
        //     // this.Button1.TabIndex = 2;
        //     // this.Button1.Text = "キャンセル";
        //     // // 
        //     // // ProgressForm
        //     // // 
        //     // this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
        //     // this.ClientSize = new System.Drawing.Size(320, 85);
        //     // this.Controls.Add(this.Button1);
        //     // this.Controls.Add(this.ProgressBar1);
        //     // this.Controls.Add(this.Label1);
        //     // this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        //     // this.MaximizeBox = false;
        //     // this.MinimizeBox = false;
        //     // this.Name = "ProgressForm";
        //     // this.ShowInTaskbar = false;
        //     // this.Text = "ProgressForm";
        //     // this.ResumeLayout(false);

        // }
        // #endregion
    }

    /// <summary>
    /// 進行状況ダイアログを表示するためのクラス
    /// </summary>
    public class ProgressDialog
    {
        //キャンセルボタンがクリックされたか
        private volatile bool _canceled = false;
        //ダイアログフォーム
        private volatile ProgressForm form;
        //フォームが表示されるまで待機するための待機ハンドル
        private System.Threading.ManualResetEvent startEvent;
        //フォームが一度表示されたか
        private bool showed = false;
        //フォームをコードで閉じているか
        private volatile bool closing = false;

        //別処理をするためのスレッド
        private System.Threading.Thread thread;

        //フォームのタイトル
        private volatile string _title = "進行状況";
        //ProgressBarの最小、最大、現在の値
        private volatile int _minimum = 0;
        private volatile int _maximum = 100;
        private volatile int _value = 0;
        //表示するメッセージ
        private volatile string old_message = "";
        private volatile string _message = "";

        private bool override_str = false;

        /// <summary>
        /// ダイアログのタイトルバーに表示する文字列
        /// </summary>
        public string Title
        {
            set
            {
                _title = value;
                if (form != null)
                    form.Dispatcher.Invoke(new MethodInvoker(SetTitle));
            }
            get
            {
                return _title;
            }
        }

        /// <summary>
        /// プログレスバーの最小値
        /// </summary>
        public int Minimum
        {
            set
            {
                _minimum = value;
                if (form != null)
                    form.Dispatcher.Invoke(new MethodInvoker(SetProgressMinimum));
            }
            get
            {
                return _minimum;
            }
        }

        /// <summary>
        /// プログレスバーの最大値
        /// </summary>
        public int Maximum
        {
            set
            {
                _maximum = value;
                if (form != null)
                    form.Dispatcher.Invoke(new MethodInvoker(SetProgressMaximun));
            }
            get
            {
                return _maximum;
            }
        }

        /// <summary>
        /// プログレスバーの値
        /// </summary>
        public int Value
        {
            set
            {
                _value = value;
                if (form != null)
                {
                    form.Dispatcher.Invoke(new MethodInvoker(SetProgressValue));
                }
            }
            get
            {
                return _value;
            }
        }

        /// <summary>
        /// ダイアログに表示するメッセージ
        /// </summary>
        public string Message
        {
            set
            {
                if (value == null)
                {
                    return;
                }

                if (value.Contains(":") && _message.Contains(":"))
                {
                    string first_word_v = value.Substring(0, value.IndexOf(":"));
                    string first_word_m = _message.Substring(0, _message.IndexOf(":"));
                    override_str = first_word_m == first_word_v;
                }
                old_message = _message;
                _message = value;
                if (form != null)
                    form.Dispatcher.Invoke(new MethodInvoker(SetMessage));
            }
            get
            {
                return _message;
            }
        }

        /// <summary>
        /// キャンセルされたか
        /// </summary>
        public bool Canceled
        {
            get { return _canceled; }
        }

        /// <summary>
        /// ダイアログを表示する
        /// </summary>
        /// <remarks>
        /// このメソッドは一回しか呼び出せません。
        /// </remarks>
        public void Show()
        {
            if (showed)
                throw new Exception("ダイアログは一度表示されています。");
            showed = true;

            _canceled = false;
            startEvent = new System.Threading.ManualResetEvent(false);

            //スレッドを作成
            thread = new System.Threading.Thread(new System.Threading.ThreadStart(Run))
            {
                IsBackground = true
            };
            thread.SetApartmentState(System.Threading.ApartmentState.STA);
            thread.Start();

            //フォームが表示されるまで待機する
            // startEvent.WaitOne();
        }

        //別スレッドで処理するメソッド
        private void Run()
        {
            //フォームの設定
            form = new ProgressForm();
            form.title_box.Text = $"Ready to {_title} ...";
            // form.Button1.Click += new EventHandler(Button1_Click);
            //form.Closing += new CancelEventHandler(form_Closing);
            //form.Activated += new EventHandler(form_Activated);
            form.ProgressBar1.Minimum = _minimum;
            form.ProgressBar1.Maximum = _maximum;
            form.ProgressBar1.Value = _value;
            //フォームの表示位置をオーナーの中央へ
            // if (ownerForm != null)
            // {
            //     form.StartPosition = FormStartPosition.Manual;
            //     form.Left =
            //         ownerForm.Left + (ownerForm.Width - form.Width) / 2;
            //     form.Top =
            //         ownerForm.Top + (ownerForm.Height - form.Height) / 2;
            // }
            //フォームの表示
            form.ShowDialog();

            form.Dispose();
        }

        /// <summary>
        /// ダイアログを閉じる
        /// </summary>
        public void Close()
        {
            closing = true;
            form.Dispatcher.Invoke(new MethodInvoker(form.Close));
        }

        // public void Dispose()
        // {
        //     form.Dispatcher.Invoke(new MethodInvoker(form.Dispose));
        // }

        private void SetProgressValue()
        {
            if (form != null)
            {
                form.bar_display.Text = $"Finish {_value}%";
                form.ProgressBar1.Value = _value;
            }
        }

        private void SetProgressText()
        {
            if (form != null)
            {
                form.bar_display.Text = $"Finish {_value}%";
            }
        }

        private void SetMessage()
        {
            if (form != null)
            {
                if (override_str && old_message != "")
                {
                    form.log.Text = form.log.Text.Replace(old_message, _message);
                }
                else
                {
                    form.log.Text += $"{_message}\n";
                }
            }
        }

        private void SetTitle()
        {
            if (form != null)
                form.title_box.Text = _title;
        }

        private void SetProgressMaximun()
        {
            if (form != null)
                form.ProgressBar1.Maximum = _maximum;
        }

        private void SetProgressMinimum()
        {
            if (form != null)
                form.ProgressBar1.Minimum = _minimum;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            _canceled = true;
        }

        private void Form_Closing(object sender, CancelEventArgs e)
        {
            if (!closing)
            {
                e.Cancel = true;
                _canceled = true;
            }
        }

        private void Form_Activated(object sender, EventArgs e)
        {
            form.Activated -= new EventHandler(Form_Activated);
            startEvent.Set();
        }

    }
}
