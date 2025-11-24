using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DATAFILTER
{
    public partial class MainForm : Form
    {
        #region KHAI BÁO CÁC BIẾN
        private readonly Color evenRowColor = Color.White;
        private readonly Color oddRowColor = Color.FromArgb(220, 220, 220);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        private const int WM_SETREDRAW = 0x000B;
        private readonly BackgroundWorker filterWorker;
        private int lastHighlightedLineInput = -1;
        private int lastHighlightedLineResult = -1;
        private const int LARGE_DATA_THRESHOLD = 5000;
        private readonly List<int> lastHighlightedLinesInput = new List<int>();
        private readonly List<int> lastHighlightedLinesResult = new List<int>();
        private bool isInputPlaceholder = true;
        private bool isResultPlaceholder = true;
        private bool suppressTextChangedEvent = false;
        private System.Threading.Timer textChangedTimer;
        private string lastExportedFilePath = string.Empty;
        #endregion

        #region PHẦN KHỞI TẠO CHÍNH
        public MainForm()
        {
            // Khởi tạo filterWorker trước
            filterWorker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true
            };

            InitializeComponent();
            InitializeCustomSettings();
            InitializeEventHandlers();
            InitializeWorkers();

            SetPlaceholder(inputTextBox, "📝 Paste dữ liệu vào đây hoặc kéo thả file...");
            SetPlaceholder(resultTextBox, "⏳ Kết quả sẽ hiển thị ở đây sau khi lọc...");

            UpdateLineCount();
            lineCountComboBox.SelectedIndex = 0;
            UpdateStatus("✔️ Sẵn sàng", false);
        }

        private void InitializeCustomSettings()
        {
            // Cấu hình textbox
            inputTextBox.WordWrap = false;
            resultTextBox.WordWrap = false;
            inputTextBox.ScrollBars = RichTextBoxScrollBars.Both;
            resultTextBox.ScrollBars = RichTextBoxScrollBars.Both;
            inputTextBox.BorderStyle = BorderStyle.None;
            resultTextBox.BorderStyle = BorderStyle.None;

            // Tắt context menu mặc định
            inputTextBox.ContextMenuStrip = null;
            resultTextBox.ContextMenuStrip = null;

            // Enable drag & drop
            inputTextBox.AllowDrop = true;

            // Thiết lập sự kiện placeholder
            SetupPlaceholderEvents();

            // Tạo context menu
            CreateContextMenus();
        }

        private void InitializeEventHandlers()
        {
            inputTextBox.TextChanged += InputTextBox_TextChanged;
            resultTextBox.TextChanged += ResultTextBox_TextChanged;
            inputTextBox.KeyDown += InputTextBox_KeyDown;
            inputTextBox.MouseClick += InputTextBox_MouseClick;
            resultTextBox.MouseClick += ResultTextBox_MouseClick;

            // Sự kiện kéo thả file
            inputTextBox.DragEnter += InputTextBox_DragEnter;
            inputTextBox.DragDrop += InputTextBox_DragDrop;

            inputTextBox.MouseDown += InputTextBox_MouseDown;
            resultTextBox.MouseDown += ResultTextBox_MouseDown;

            // ✅ THÊM EVENT CHO STATUS LABEL
            statusLabel.Click += StatusLabel_Click;
        }

        private void InitializeWorkers()
        {
            filterWorker.DoWork += FilterWorker_DoWork;
            filterWorker.RunWorkerCompleted += FilterWorker_RunWorkerCompleted;
        }
        #endregion

        #region SỰ KIỆN CHO PLACEHOLDER
        private void SetupPlaceholderEvents()
        {
            // Sự kiện cho inputTextBox
            inputTextBox.GotFocus += (sender, e) =>
            {
                if (isInputPlaceholder)
                {
                    inputTextBox.Clear();
                    inputTextBox.ForeColor = Color.Black;
                    inputTextBox.SelectAll();
                    inputTextBox.SelectionFont = new Font(inputTextBox.Font, FontStyle.Regular);
                    inputTextBox.DeselectAll();
                    isInputPlaceholder = false;
                }
            };

            inputTextBox.LostFocus += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(inputTextBox.Text))
                {
                    inputTextBox.Text = "📝 Paste dữ liệu vào đây hoặc kéo thả file...";
                    inputTextBox.ForeColor = Color.Gray;
                    inputTextBox.SelectAll();
                    inputTextBox.SelectionFont = new Font(inputTextBox.Font, FontStyle.Italic);
                    inputTextBox.DeselectAll();
                    isInputPlaceholder = true;
                }
            };

            // Sự kiện cho resultTextBox
            resultTextBox.GotFocus += (sender, e) =>
            {
                if (isResultPlaceholder)
                {
                    resultTextBox.Clear();
                    resultTextBox.ForeColor = Color.Black;
                    resultTextBox.SelectAll();
                    resultTextBox.SelectionFont = new Font(resultTextBox.Font, FontStyle.Regular);
                    resultTextBox.DeselectAll();
                    isResultPlaceholder = false;
                }
            };

            resultTextBox.LostFocus += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(resultTextBox.Text))
                {
                    resultTextBox.Text = "⏳ Kết quả sẽ hiển thị ở đây sau khi lọc...";
                    resultTextBox.ForeColor = Color.Gray;
                    resultTextBox.SelectAll();
                    resultTextBox.SelectionFont = new Font(resultTextBox.Font, FontStyle.Italic);
                    resultTextBox.DeselectAll();
                    isResultPlaceholder = true;
                }
            };
        }
        #endregion

        #region XỬ LÝ MOUSEDOWN ĐỂ XÓA PLACEHOLDER
        private void InputTextBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (isInputPlaceholder)
            {
                inputTextBox.Clear();
                inputTextBox.ForeColor = Color.Black;
                inputTextBox.SelectAll();
                inputTextBox.SelectionFont = new Font(inputTextBox.Font, FontStyle.Regular);
                inputTextBox.DeselectAll();
                isInputPlaceholder = false;
            }
        }

        private void ResultTextBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (isResultPlaceholder)
            {
                resultTextBox.Clear();
                resultTextBox.ForeColor = Color.Black;
                resultTextBox.SelectAll();
                resultTextBox.SelectionFont = new Font(resultTextBox.Font, FontStyle.Regular);
                resultTextBox.DeselectAll();
                isResultPlaceholder = false;
            }
        }
        #endregion

        #region PHƯƠNG THỨC DRAG & DROP
        private void InputTextBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void InputTextBox_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    if (files.Length > 0)
                    {
                        string fileContent = File.ReadAllText(files[0], Encoding.UTF8);
                        fileContent = fileContent.Replace("\r\n", "\n").Replace("\r", "\n").Trim();
                        SetTextWithIndent(inputTextBox, fileContent);
                        inputTextBox.ForeColor = Color.Black;
                        isInputPlaceholder = false;
                        UpdateStatus($"✅ Đã tải file: {Path.GetFileName(files[0])}", false);
                    }
                }
                else if (e.Data.GetDataPresent(DataFormats.Text))
                {
                    string text = (string)e.Data.GetData(DataFormats.Text);
                    text = text.Replace("\r\n", "\n").Replace("\r", "\n").Trim();
                    SetTextWithIndent(inputTextBox, text);
                    inputTextBox.ForeColor = Color.Black;
                    isInputPlaceholder = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đọc file: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region STATUS BAR UPDATE
        private void UpdateStatus(string message, bool showProgress)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateStatus(message, showProgress)));
                return;
            }

            statusLabel.Text = message;
            statusLabel.IsLink = false; // ✅ Tắt link mode
            statusLabel.ForeColor = Color.Gray;
            progressBar.Visible = showProgress;
            if (showProgress)
            {
                progressBar.Style = ProgressBarStyle.Marquee;
            }
        }

        // ✅ THÊM PHƯƠNG THỨC MỚI
        private void UpdateStatusWithFileLink(string message, bool showProgress)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateStatusWithFileLink(message, showProgress)));
                return;
            }

            statusLabel.Text = message;
            statusLabel.IsLink = true; // ✅ Bật link mode
            statusLabel.ForeColor = Color.Blue;
            progressBar.Visible = showProgress;
        }

        private void StatusLabel_Click(object sender, EventArgs e)
        {
            // Chỉ mở khi có link và file đã được xuất
            if (statusLabel.IsLink && !string.IsNullOrEmpty(lastExportedFilePath))
            {
                OpenFileLocation(lastExportedFilePath);
            }
        }
        #endregion

        #region TẠO MENU CHUỘT PHẢI
        private void CreateContextMenus()
        {
            // Context menu cho inputTextBox
            ContextMenuStrip inputMenu = new ContextMenuStrip
            {
                RenderMode = ToolStripRenderMode.Professional
            };

            ToolStripMenuItem pasteItem = new ToolStripMenuItem("📋 Dán từ clipboard", null, (s, e) =>
            {
                try
                {
                    string pastedText = Clipboard.GetText();
                    pastedText = pastedText.Replace("\r\n", "\n").Replace("\r", "\n").Trim();
                    SetTextWithIndent(inputTextBox, pastedText);
                    inputTextBox.ForeColor = Color.Black;
                    isInputPlaceholder = false;
                    UpdateStatus("✅ Đã dán dữ liệu từ clipboard", false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi paste: {ex.Message}");
                }
            });

            ToolStripMenuItem clearItem = new ToolStripMenuItem("🗑️ Làm sạch", null, (s, e) =>
            {
                inputTextBox.Clear();
                SetPlaceholder(inputTextBox, "📝 Paste dữ liệu vào đây hoặc kéo thả file...");
                isInputPlaceholder = true;
                UpdateLineCount();
                UpdateStatus("🧹 Đã xóa dữ liệu đầu vào", false);
            });

            inputMenu.Items.Add(pasteItem);
            inputMenu.Items.Add(new ToolStripSeparator());
            inputMenu.Items.Add(clearItem);
            inputTextBox.ContextMenuStrip = inputMenu;

            // Context menu cho resultTextBox
            ContextMenuStrip resultMenu = new ContextMenuStrip
            {
                RenderMode = ToolStripRenderMode.Professional
            };

            ToolStripMenuItem copyItem = new ToolStripMenuItem("📋 Sao chép kết quả", null, (s, e) =>
            {
                try
                {
                    if (!isResultPlaceholder && !string.IsNullOrWhiteSpace(resultTextBox.Text))
                    {
                        Clipboard.SetText(resultTextBox.Text);
                        UpdateStatus("✅ Đã sao chép kết quả vào clipboard", false);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi sao chép: {ex.Message}");
                }
            });

            ToolStripMenuItem exportItem = new ToolStripMenuItem("💾 Xuất thành file", null, (s, e) =>
            {
                ExportButton_Click(null, null);
            });

            ToolStripMenuItem clearResultItem = new ToolStripMenuItem("🗑️ Làm sạch", null, (s, e) =>
            {
                resultTextBox.Clear();
                SetPlaceholder(resultTextBox, "⏳ Kết quả sẽ hiển thị ở đây sau khi lọc...");
                isResultPlaceholder = true;
                UpdateLineCount();
                UpdateStatus("🧹 Đã xóa kết quả", false);
            });

            resultMenu.Items.Add(copyItem);
            resultMenu.Items.Add(new ToolStripSeparator());
            resultMenu.Items.Add(exportItem);
            resultMenu.Items.Add(new ToolStripSeparator());
            resultMenu.Items.Add(clearResultItem);
            resultTextBox.ContextMenuStrip = resultMenu;
        }
        #endregion

        #region PLACEHOLDER VÀ PASTE HANDLER
        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                e.Handled = true;
                try
                {
                    string pastedText = Clipboard.GetText();
                    pastedText = pastedText.Replace("\r\n", "\n").Replace("\r", "\n").Trim();
                    SetTextWithIndent(inputTextBox, pastedText);
                    inputTextBox.ForeColor = Color.Black;
                    isInputPlaceholder = false;
                    UpdateStatus("✅ Đã dán dữ liệu", false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi paste: {ex.Message}");
                }
            }
        }

        private void SetPlaceholder(RichTextBox textBox, string placeholder)
        {
            textBox.Text = placeholder;
            textBox.ForeColor = Color.Gray;
            textBox.SelectAll();
            textBox.SelectionFont = new Font(textBox.Font, FontStyle.Italic);
            textBox.DeselectAll();

            textBox.GotFocus += (sender, e) =>
            {
                if ((textBox == inputTextBox && isInputPlaceholder) ||
                    (textBox == resultTextBox && isResultPlaceholder))
                {
                    textBox.Clear();
                    textBox.ForeColor = Color.Black;
                    textBox.SelectAll();
                    textBox.SelectionFont = new Font(textBox.Font, FontStyle.Regular);
                    textBox.DeselectAll();

                    // Cập nhật trạng thái placeholder
                    if (textBox == inputTextBox)
                        isInputPlaceholder = true;
                    else if (textBox == resultTextBox)
                        isResultPlaceholder = true;
                }
            };

            textBox.LostFocus += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholder;
                    textBox.ForeColor = Color.Gray;
                    textBox.SelectAll();
                    textBox.SelectionFont = new Font(textBox.Font, FontStyle.Italic);
                    textBox.DeselectAll();

                    if (textBox == inputTextBox)
                        isInputPlaceholder = true;
                    else
                        isResultPlaceholder = true;
                }
            };
        }

        private void InputTextBox_TextChanged(object sender, EventArgs e)
        {
            if (suppressTextChangedEvent) return;
            UpdateLineCount();

            textChangedTimer?.Dispose();
            textChangedTimer = new System.Threading.Timer(_ =>
            {
                this.Invoke(new Action(() =>
                {
                    if (inputTextBox.Lines.Length < 500)
                    {
                        ApplyAlternatingColors();
                    }
                }));
            }, null, 200, Timeout.Infinite);
        }

        private void ResultTextBox_TextChanged(object sender, EventArgs e)
        {
            if (suppressTextChangedEvent) return;
            UpdateLineCount();
        }

        private void SetTextWithIndent(RichTextBox textBox, string text, bool moveCursorToEnd = true)
        {
            textBox.SuspendLayout();
            int currentPosition = textBox.SelectionStart;
            int currentLength = textBox.SelectionLength;

            textBox.Text = text + Environment.NewLine;
            textBox.SelectAll();
            textBox.SelectionIndent = 0;
            textBox.SelectionRightIndent = 0;

            if (moveCursorToEnd)
            {
                textBox.Select(textBox.Text.Length, 0);
                textBox.ScrollToCaret();
            }
            else
            {
                if (currentPosition <= textBox.Text.Length)
                {
                    textBox.Select(currentPosition, Math.Min(currentLength, textBox.Text.Length - currentPosition));
                }
                else
                {
                    textBox.Select(0, 0);
                }
            }
            textBox.ResumeLayout();
        }
        #endregion

        #region TƯƠNG TÁC CLICK ĐỂ HIGHLIGHT
        private void InputTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (isInputPlaceholder) return;

            try
            {
                int clickPosition = inputTextBox.GetCharIndexFromPosition(e.Location);
                int lineIndex = inputTextBox.GetLineFromCharIndex(clickPosition);
                string[] lines = inputTextBox.Lines;

                if (lineIndex < 0 || lineIndex >= lines.Length) return;
                string clickedLine = lines[lineIndex].Trim();
                if (string.IsNullOrWhiteSpace(clickedLine)) return;

                string key = ExtractKeyFromLine(clickedLine);
                if (string.IsNullOrWhiteSpace(key)) return;

                if (inputTextBox.Lines.Length > LARGE_DATA_THRESHOLD)
                {
                    HighlightLineWithColorAsync(inputTextBox, lineIndex, true);
                    HighlightResultLinesByKeyAsync(key);
                }
                else
                {
                    HighlightLineWithColor(inputTextBox, lineIndex, true);
                    HighlightResultLinesByKey(key);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi InputTextBox_MouseClick: {ex.Message}");
            }
        }

        private void ResultTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (isResultPlaceholder) return;

            try
            {
                int clickPosition = resultTextBox.GetCharIndexFromPosition(e.Location);
                int lineIndex = resultTextBox.GetLineFromCharIndex(clickPosition);
                string[] lines = resultTextBox.Lines;

                if (lineIndex < 0 || lineIndex >= lines.Length) return;
                string clickedLine = lines[lineIndex].Trim();
                if (string.IsNullOrWhiteSpace(clickedLine)) return;

                string key = ExtractKeyFromLine(clickedLine);
                if (string.IsNullOrWhiteSpace(key)) return;

                if (resultTextBox.Lines.Length > LARGE_DATA_THRESHOLD)
                {
                    HighlightLineWithColorAsync(resultTextBox, lineIndex, false);
                    HighlightInputLinesByKeyAsync(key);
                }
                else
                {
                    HighlightLineWithColor(resultTextBox, lineIndex, false);
                    HighlightInputLinesByKey(key);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi ResultTextBox_MouseClick: {ex.Message}");
            }
        }

        private string ExtractKeyFromLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return null;
            line = line.Trim();

            if (line.Contains("\t"))
            {
                int tabIndex = line.IndexOf('\t');
                return line.Substring(0, tabIndex).Trim();
            }

            if (line.Contains("="))
            {
                int eqIndex = line.IndexOf('=');
                return line.Substring(0, eqIndex).Trim();
            }

            return null;
        }

        private void HighlightLineWithColor(RichTextBox textBox, int lineIndex, bool isInputBox)
        {
            try
            {
                if (lineIndex < 0 || lineIndex >= textBox.Lines.Length) return;

                suppressTextChangedEvent = true;
                int currentPosition = textBox.SelectionStart;
                int currentLength = textBox.SelectionLength;

                List<int> lastHighlightedLines = isInputBox ? lastHighlightedLinesInput : lastHighlightedLinesResult;

                foreach (int oldLine in lastHighlightedLines)
                {
                    if (oldLine >= 0 && oldLine < textBox.Lines.Length)
                    {
                        int oldStartIndex = textBox.GetFirstCharIndexFromLine(oldLine);
                        if (oldStartIndex >= 0)
                        {
                            textBox.Select(oldStartIndex, textBox.Lines[oldLine].Length);
                            textBox.SelectionBackColor = textBox.BackColor;
                        }
                    }
                }

                if (isInputBox)
                    lastHighlightedLinesInput.Clear();
                else
                    lastHighlightedLinesResult.Clear();

                int startIndex = textBox.GetFirstCharIndexFromLine(lineIndex);
                if (startIndex >= 0)
                {
                    textBox.Select(startIndex, textBox.Lines[lineIndex].Length);
                    textBox.SelectionBackColor = Color.FromArgb(255, 235, 59);

                    EnsureLineIsVisible(textBox, lineIndex);
                }

                if (currentPosition <= textBox.Text.Length)
                {
                    textBox.Select(currentPosition, Math.Min(currentLength, textBox.Text.Length - currentPosition));
                }

                if (isInputBox)
                {
                    lastHighlightedLineInput = lineIndex;
                    lastHighlightedLinesInput.Add(lineIndex);
                }
                else
                {
                    lastHighlightedLineResult = lineIndex;
                    lastHighlightedLinesResult.Add(lineIndex);
                }

                suppressTextChangedEvent = false;
            }
            catch (Exception ex)
            {
                suppressTextChangedEvent = false;
                System.Diagnostics.Debug.WriteLine($"Lỗi HighlightLineWithColor: {ex.Message}");
            }
        }

        private void EnsureLineIsVisible(RichTextBox textBox, int lineIndex)
        {
            if (lineIndex < 0 || lineIndex >= textBox.Lines.Length) return;

            SendMessage(textBox.Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);

            try
            {
                int visibleLines = GetVisibleLineCount(textBox);
                if (visibleLines <= 0) return;

                int targetLine = Math.Max(0, lineIndex - visibleLines / 2);
                int totalLines = textBox.Lines.Length;
                if (targetLine + visibleLines > totalLines)
                {
                    targetLine = Math.Max(0, totalLines - visibleLines);
                }

                int targetCharIndex = textBox.GetFirstCharIndexFromLine(targetLine);
                if (targetCharIndex >= 0)
                {
                    textBox.Select(targetCharIndex, 0);
                    textBox.ScrollToCaret();
                }
            }
            finally
            {
                SendMessage(textBox.Handle, WM_SETREDRAW, (IntPtr)1, IntPtr.Zero);
                textBox.Invalidate();
            }
        }

        private int GetVisibleLineCount(RichTextBox textBox)
        {
            using (Graphics g = textBox.CreateGraphics())
            {
                int clientHeight = textBox.ClientSize.Height - textBox.Padding.Top - textBox.Padding.Bottom;
                int lineHeight = TextRenderer.MeasureText(g, "A", textBox.Font).Height;
                if (lineHeight == 0) return 10;
                return clientHeight / lineHeight;
            }
        }

        private void HighlightLineWithColorAsync(RichTextBox textBox, int lineIndex, bool isInputBox)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    if (lineIndex >= 0 && lineIndex < textBox.Lines.Length)
                    {
                        this.Invoke(new Action(() =>
                        {
                            try
                            {
                                List<int> lastHighlightedLines = isInputBox ? lastHighlightedLinesInput : lastHighlightedLinesResult;

                                foreach (int oldLine in lastHighlightedLines)
                                {
                                    if (oldLine >= 0 && oldLine < textBox.Lines.Length)
                                    {
                                        int oldStartIndex = textBox.GetFirstCharIndexFromLine(oldLine);
                                        if (oldStartIndex >= 0)
                                        {
                                            textBox.Select(oldStartIndex, textBox.Lines[oldLine].Length);
                                            textBox.SelectionBackColor = Color.White;
                                        }
                                    }
                                }

                                if (isInputBox)
                                    lastHighlightedLinesInput.Clear();
                                else
                                    lastHighlightedLinesResult.Clear();

                                int startIndex = textBox.GetFirstCharIndexFromLine(lineIndex);
                                if (startIndex >= 0)
                                {
                                    textBox.Select(startIndex, textBox.Lines[lineIndex].Length);
                                    textBox.SelectionBackColor = Color.Yellow;
                                    textBox.ScrollToCaret();
                                }

                                if (isInputBox)
                                {
                                    lastHighlightedLineInput = lineIndex;
                                    lastHighlightedLinesInput.Add(lineIndex);
                                }
                                else
                                {
                                    lastHighlightedLineResult = lineIndex;
                                    lastHighlightedLinesResult.Add(lineIndex);
                                }
                            }
                            catch { }
                        }));
                    }
                }
                catch { }
            });
        }

        private void HighlightResultLinesByKey(string key)
        {
            try
            {
                if (isResultPlaceholder) return;

                string[] resultLines = resultTextBox.Lines;
                List<int> matchingLines = new List<int>();

                for (int i = 0; i < resultLines.Length; i++)
                {
                    string lineKey = ExtractKeyFromLine(resultLines[i]);
                    if (!string.IsNullOrWhiteSpace(lineKey) && lineKey.Equals(key, StringComparison.OrdinalIgnoreCase))
                    {
                        matchingLines.Add(i);
                    }
                }

                foreach (int oldLine in lastHighlightedLinesResult)
                {
                    if (oldLine >= 0 && oldLine < resultTextBox.Lines.Length)
                    {
                        int oldStartIndex = resultTextBox.GetFirstCharIndexFromLine(oldLine);
                        if (oldStartIndex >= 0)
                        {
                            resultTextBox.Select(oldStartIndex, resultTextBox.Lines[oldLine].Length);
                            resultTextBox.SelectionBackColor = resultTextBox.BackColor;
                        }
                    }
                }

                if (matchingLines.Count > 0)
                {
                    foreach (int lineIndex in matchingLines)
                    {
                        int startIndex = resultTextBox.GetFirstCharIndexFromLine(lineIndex);
                        if (startIndex >= 0)
                        {
                            resultTextBox.Select(startIndex, resultTextBox.Lines[lineIndex].Length);
                            resultTextBox.SelectionBackColor = Color.FromArgb(255, 235, 59);
                        }
                    }

                    EnsureLineIsVisible(resultTextBox, matchingLines[0]);
                    lastHighlightedLineResult = matchingLines[0];
                    lastHighlightedLinesResult.Clear();
                    lastHighlightedLinesResult.AddRange(matchingLines);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi HighlightResultLinesByKey: {ex.Message}");
            }
        }

        private void HighlightResultLinesByKeyAsync(string key)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    if (isResultPlaceholder) return;

                    string[] resultLines = resultTextBox.Lines;
                    List<int> matchingLines = new List<int>();

                    for (int i = 0; i < resultLines.Length; i++)
                    {
                        string lineKey = ExtractKeyFromLine(resultLines[i]);
                        if (!string.IsNullOrWhiteSpace(lineKey) && lineKey.Equals(key, StringComparison.OrdinalIgnoreCase))
                        {
                            matchingLines.Add(i);
                        }
                    }

                    if (matchingLines.Count > 0)
                    {
                        this.Invoke(new Action(() =>
                        {
                            try
                            {
                                foreach (int oldLine in lastHighlightedLinesResult)
                                {
                                    if (oldLine >= 0 && oldLine < resultTextBox.Lines.Length)
                                    {
                                        int oldStartIndex = resultTextBox.GetFirstCharIndexFromLine(oldLine);
                                        if (oldStartIndex >= 0)
                                        {
                                            resultTextBox.Select(oldStartIndex, resultTextBox.Lines[oldLine].Length);
                                            resultTextBox.SelectionBackColor = Color.White;
                                        }
                                    }
                                }

                                foreach (int lineIndex in matchingLines)
                                {
                                    int startIndex = resultTextBox.GetFirstCharIndexFromLine(lineIndex);
                                    if (startIndex >= 0)
                                    {
                                        resultTextBox.Select(startIndex, resultTextBox.Lines[lineIndex].Length);
                                        resultTextBox.SelectionBackColor = Color.Yellow;
                                    }
                                }

                                int firstLineStart = resultTextBox.GetFirstCharIndexFromLine(matchingLines[0]);
                                if (firstLineStart >= 0)
                                {
                                    resultTextBox.Select(firstLineStart, 0);
                                    resultTextBox.ScrollToCaret();
                                }

                                lastHighlightedLineResult = matchingLines[0];
                                lastHighlightedLinesResult.Clear();
                                lastHighlightedLinesResult.AddRange(matchingLines);
                            }
                            catch { }
                        }));
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Lỗi HighlightResultLinesByKeyAsync: {ex.Message}");
                }
            });
        }

        private void HighlightInputLinesByKey(string key)
        {
            try
            {
                if (isInputPlaceholder) return;

                string[] inputLines = inputTextBox.Lines;
                List<int> matchingLines = new List<int>();

                for (int i = 0; i < inputLines.Length; i++)
                {
                    string lineKey = ExtractKeyFromLine(inputLines[i]);
                    if (!string.IsNullOrWhiteSpace(lineKey) && lineKey.Equals(key, StringComparison.OrdinalIgnoreCase))
                    {
                        matchingLines.Add(i);
                    }
                }

                foreach (int oldLine in lastHighlightedLinesInput)
                {
                    if (oldLine >= 0 && oldLine < inputTextBox.Lines.Length)
                    {
                        int oldStartIndex = inputTextBox.GetFirstCharIndexFromLine(oldLine);
                        if (oldStartIndex >= 0)
                        {
                            inputTextBox.Select(oldStartIndex, inputTextBox.Lines[oldLine].Length);
                            inputTextBox.SelectionBackColor = inputTextBox.BackColor;
                        }
                    }
                }

                if (matchingLines.Count > 0)
                {
                    foreach (int lineIndex in matchingLines)
                    {
                        int startIndex = inputTextBox.GetFirstCharIndexFromLine(lineIndex);
                        if (startIndex >= 0)
                        {
                            inputTextBox.Select(startIndex, inputTextBox.Lines[lineIndex].Length);
                            inputTextBox.SelectionBackColor = Color.FromArgb(255, 235, 59);
                        }
                    }

                    EnsureLineIsVisible(inputTextBox, matchingLines[0]);
                    lastHighlightedLineInput = matchingLines[0];
                    lastHighlightedLinesInput.Clear();
                    lastHighlightedLinesInput.AddRange(matchingLines);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi HighlightInputLinesByKey: {ex.Message}");
            }
        }

        private void HighlightInputLinesByKeyAsync(string key)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    if (isInputPlaceholder) return;

                    string[] inputLines = inputTextBox.Lines;
                    List<int> matchingLines = new List<int>();

                    for (int i = 0; i < inputLines.Length; i++)
                    {
                        string lineKey = ExtractKeyFromLine(inputLines[i]);
                        if (!string.IsNullOrWhiteSpace(lineKey) && lineKey.Equals(key, StringComparison.OrdinalIgnoreCase))
                        {
                            matchingLines.Add(i);
                        }
                    }

                    if (matchingLines.Count > 0)
                    {
                        this.Invoke(new Action(() =>
                        {
                            try
                            {
                                foreach (int oldLine in lastHighlightedLinesInput)
                                {
                                    if (oldLine >= 0 && oldLine < inputTextBox.Lines.Length)
                                    {
                                        int oldStartIndex = inputTextBox.GetFirstCharIndexFromLine(oldLine);
                                        if (oldStartIndex >= 0)
                                        {
                                            inputTextBox.Select(oldStartIndex, inputTextBox.Lines[oldLine].Length);
                                            inputTextBox.SelectionBackColor = Color.White;
                                        }
                                    }
                                }

                                foreach (int lineIndex in matchingLines)
                                {
                                    int startIndex = inputTextBox.GetFirstCharIndexFromLine(lineIndex);
                                    if (startIndex >= 0)
                                    {
                                        inputTextBox.Select(startIndex, inputTextBox.Lines[lineIndex].Length);
                                        inputTextBox.SelectionBackColor = Color.Yellow;
                                    }
                                }

                                int firstLineStart = inputTextBox.GetFirstCharIndexFromLine(matchingLines[0]);
                                if (firstLineStart >= 0)
                                {
                                    inputTextBox.Select(firstLineStart, 0);
                                    inputTextBox.ScrollToCaret();
                                }

                                lastHighlightedLineInput = matchingLines[0];
                                lastHighlightedLinesInput.Clear();
                                lastHighlightedLinesInput.AddRange(matchingLines);
                            }
                            catch { }
                        }));
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Lỗi HighlightInputLinesByKeyAsync: {ex.Message}");
                }
            });
        }
        #endregion

        #region CẬP NHẬT SỐ DÒNG
        private void UpdateLineCount()
        {
            int inputLines = 0;
            if (!isInputPlaceholder && !string.IsNullOrWhiteSpace(inputTextBox.Text))
            {
                inputLines = inputTextBox.Lines.Count(line => !string.IsNullOrWhiteSpace(line));
            }

            int resultLines = 0;
            if (!isResultPlaceholder && !string.IsNullOrWhiteSpace(resultTextBox.Text))
            {
                resultLines = resultTextBox.Lines.Count(line => !string.IsNullOrWhiteSpace(line));
            }

            inputCountLabel.Text = $"📊 Số lượng: {inputLines} dòng";
            resultCountLabel.Text = $"✅ Kết quả: {resultLines} dòng";
        }
        #endregion

        #region XỬ LÝ LỌC DỮ LIỆU
        private void ApplyAlternatingColors()
        {
            // Tắt chức năng vẽ màu nền xen kẽ
            return;
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            if (filterWorker.IsBusy)
            {
                MessageBox.Show("⏳ Đang xử lý dữ liệu, vui lòng đợi...", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (isInputPlaceholder || string.IsNullOrWhiteSpace(inputTextBox.Text))
            {
                MessageBox.Show("⚠️ Vui lòng nhập dữ liệu trước khi lọc!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            filterButton.Enabled = false;
            filterButton.Text = "⏳ ĐANG XỬ LÝ...";
            Cursor = Cursors.WaitCursor;
            UpdateStatus("🔄 Đang lọc dữ liệu...", true);

            int lineCount = lineCountComboBox.SelectedIndex == 0 ? 1 :
                            lineCountComboBox.SelectedIndex == 1 ? 2 : 3;

            string[] lines = inputTextBox.Text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            filterWorker.RunWorkerAsync(new { Lines = lines, LineCount = lineCount });
        }

        private void FilterWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var args = e.Argument as dynamic;
            string[] lines = args.Lines;
            int lineCount = args.LineCount;
            var result = FilterData(lines, lineCount);
            e.Result = result;
        }

        private void FilterWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            filterButton.Enabled = true;
            filterButton.Text = "🔍 LỌC DỮ LIỆU";
            Cursor = Cursors.Default;

            if (e.Error != null)
            {
                MessageBox.Show($"❌ Lỗi khi lọc dữ liệu: {e.Error.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus("❌ Lỗi khi lọc dữ liệu", false);
                return;
            }

            if (e.Result is List<string> filteredData && filteredData.Count > 0)
            {
                resultTextBox.SuspendLayout();
                resultTextBox.Clear();
                resultTextBox.ForeColor = Color.Black;
                isResultPlaceholder = false;

                string resultText = string.Join(Environment.NewLine, filteredData);
                SetTextWithIndent(resultTextBox, resultText);

                ApplyAlternatingColors();
                resultTextBox.ResumeLayout();
                UpdateStatus($"✅ Lọc thành công {filteredData.Count} dòng", false);
            }
            else
            {
                UpdateStatus("⚠️ Không có dữ liệu sau khi lọc", false);
            }
            UpdateLineCount();
        }

        private List<string> FilterData(string[] lines, int lineCount)
        {
            var result = new List<string>();

            bool isTabFormat = false;
            if (lines.Length > 0)
            {
                isTabFormat = lines[0].Contains("\t");
            }

            var groups = isTabFormat
                ? lines
                    .Select(line => line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries))
                    .Where(parts => parts.Length == 3)
                    .GroupBy(
                        parts => parts[0],
                        parts =>
                        {
                            int.TryParse(parts[1], out int val1);
                            int.TryParse(parts[2], out int val2);
                            return new int[] { val1, val2 };
                        }
                    )
                : lines
                    .Select(line => line.Split('='))
                    .Where(parts => parts.Length == 2)
                    .GroupBy(
                        parts => parts[0],
                        parts =>
                        {
                            var values = parts[1].Split(',');
                            int.TryParse(values[0], out int val1);
                            int.TryParse(values[1], out int val2);
                            return new int[] { val1, val2 };
                        }
                    );

            foreach (var group in groups)
            {
                var key = group.Key;
                var values = group.ToList();

                if (values.Count == 1)
                {
                    if (isTabFormat)
                        result.Add($"{key}\t{values[0][0]}\t{values[0][1]}");
                    else
                        result.Add($"{key}={values[0][0]},{values[0][1]}");
                    continue;
                }

                var selectedIndices = SelectDifferentLines(values, lineCount);

                foreach (var idx in selectedIndices)
                {
                    if (isTabFormat)
                        result.Add($"{key}\t{values[idx][0]}\t{values[idx][1]}");
                    else
                        result.Add($"{key}={values[idx][0]},{values[idx][1]}");
                }
            }

            return result;
        }

        private List<int> SelectDifferentLines(List<int[]> values, int lineCount)
        {
            if (lineCount == 1)
            {
                var distances = new List<(int index, double maxDist)>();

                for (int i = 0; i < values.Count; i++)
                {
                    double currentMax = 0.0;
                    for (int j = 0; j < values.Count; j++)
                    {
                        if (i != j)
                        {
                            double distance = Math.Sqrt(
                                Math.Pow(values[i][0] - values[j][0], 2) +
                                Math.Pow(values[i][1] - values[j][1], 2)
                            );
                            if (distance > currentMax)
                            {
                                currentMax = distance;
                            }
                        }
                    }
                    distances.Add((i, currentMax));
                }

                return distances
                    .OrderByDescending(d => d.maxDist)
                    .Take(1)
                    .Select(d => d.index)
                    .ToList();
            }

            var selected = new List<int>();
            var remaining = Enumerable.Range(0, values.Count).ToList();

            double maxAvgDist = -1;
            int firstIndex = 0;

            foreach (var i in remaining)
            {
                double totalDist = 0;
                int count = 0;

                foreach (var j in remaining)
                {
                    if (i != j)
                    {
                        double distance = Math.Sqrt(
                            Math.Pow(values[i][0] - values[j][0], 2) +
                            Math.Pow(values[i][1] - values[j][1], 2)
                        );
                        totalDist += distance;
                        count++;
                    }
                }

                double avgDist = count > 0 ? totalDist / count : 0;
                if (avgDist > maxAvgDist)
                {
                    maxAvgDist = avgDist;
                    firstIndex = i;
                }
            }

            selected.Add(firstIndex);
            remaining.Remove(firstIndex);

            double minDistanceThreshold = 5.0;

            while (selected.Count < lineCount && remaining.Count > 0)
            {
                double maxMinDist = -1;
                int nextIndex = -1;

                foreach (var i in remaining)
                {
                    double minDist = double.MaxValue;

                    foreach (var selectedIdx in selected)
                    {
                        double distance = Math.Sqrt(
                            Math.Pow(values[i][0] - values[selectedIdx][0], 2) +
                            Math.Pow(values[i][1] - values[selectedIdx][1], 2)
                        );

                        if (distance < minDist)
                        {
                            minDist = distance;
                        }
                    }

                    if (minDist > maxMinDist)
                    {
                        maxMinDist = minDist;
                        nextIndex = i;
                    }
                }

                if (nextIndex != -1 && maxMinDist >= minDistanceThreshold)
                {
                    selected.Add(nextIndex);
                    remaining.Remove(nextIndex);
                }
                else
                {
                    break;
                }
            }

            return selected.OrderBy(idx => idx).ToList();
        }
        #endregion

        #region NÚT CLEAR VÀ EXPORT
        private void ClearButton_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("⚠️ Bạn có chắc muốn xóa toàn bộ dữ liệu?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                inputTextBox.Clear();
                resultTextBox.Clear();
                SetPlaceholder(inputTextBox, "📝 Paste dữ liệu vào đây hoặc kéo thả file...");
                SetPlaceholder(resultTextBox, "⏳ Kết quả sẽ hiển thị ở đây sau khi lọc...");
                isInputPlaceholder = true;
                isResultPlaceholder = true;
                UpdateLineCount();
                UpdateStatus("🧹 Đã xóa toàn bộ dữ liệu", false);
            }
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            if (isResultPlaceholder || string.IsNullOrWhiteSpace(resultTextBox.Text))
            {
                MessageBox.Show("⚠️ Không có dữ liệu để xuất!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                saveDialog.DefaultExt = "txt";
                saveDialog.FileName = $"TOA_DO_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                saveDialog.Title = "💾 Xuất dữ liệu ra file";

                string defaultFolder = @"D:\Non_Documents";
                if (!Directory.Exists(defaultFolder))
                {
                    try { Directory.CreateDirectory(defaultFolder); }
                    catch { defaultFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); }
                }

                saveDialog.InitialDirectory = defaultFolder;

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string textToSave = resultTextBox.Text.Replace("\n", Environment.NewLine);
                        File.WriteAllText(saveDialog.FileName, textToSave, Encoding.UTF8);

                        // ✅ LƯU ĐƯỜNG DẪN FILE
                        lastExportedFilePath = saveDialog.FileName;

                        // ✅ CẬP NHẬT STATUS VỚI LINK
                        UpdateStatusWithFileLink($"✅ Đã xuất file: {Path.GetFileName(saveDialog.FileName)} (Click để mở)", false);

                        var result = MessageBox.Show("✅ Xuất file thành công!\n\n📁 Bạn có muốn mở thư mục chứa file?",
                            "Thành công", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                        if (result == DialogResult.Yes)
                        {
                            OpenFileLocation(saveDialog.FileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"❌ Lỗi khi xuất file: {ex.Message}", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        UpdateStatus("❌ Xuất file thất bại", false);
                    }
                }
            }
        }

        private void OpenFileLocation(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{filePath}\"");
                }
                else
                {
                    MessageBox.Show("❌ File không tồn tại!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Không thể mở thư mục: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }

    #region CLASS MỞ RỘNG
    public static class RichTextBoxExtensions
    {
        public static int GetFirstVisibleLineIndex(this RichTextBox rtb)
        {
            return rtb.GetLineFromCharIndex(rtb.GetCharIndexFromPosition(new Point(0, 0)));
        }
    }

    public class CustomRichTextBox : RichTextBox
    {
        public CustomRichTextBox()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.UserPaint |
                         ControlStyles.DoubleBuffer, true);
        }
    }
    #endregion
}