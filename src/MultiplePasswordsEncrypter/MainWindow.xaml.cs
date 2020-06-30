using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static MultiplePasswordsEncrypter.MPECore;

namespace MultiplePasswordsEncrypter
{
    /// <summary>
    /// MainWindow.xaml 
    /// UI, Input Check
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            CultureInfo ci = CultureInfo.CurrentUICulture;
            if (ci.Name == "ja-JP")
            {
                MenuLangJa.IsChecked = true;
            }
            else
            {
                MenuLangEn.IsChecked = true;
            }

            EncAddQuestionButton_Click(null, null); // set first question.
        }

        #region UIConst
        private static readonly Thickness TH_BOADER = new Thickness(1);
        private static readonly Thickness TH_PADDING_5 = new Thickness(5);
        private static readonly Thickness TH_MARGIN_L10 = new Thickness(10, 0, 0, 0);
        private static readonly Thickness TH_MARGIN_T5_R5 = new Thickness(0, 5, 5, 0);
        private static readonly Thickness TH_MARGIN_T5 = new Thickness(0, 5, 0, 0);

        private const double HEADER_WIDTH = 80;
        private const double HINT_WIDTH = 200;
        private const double HINT_REMOVE_BUTTON_WIDTH = 20;
        private const double PASSWORD_WIDTH = 80;
        private const double PASSWORD_ADD_BUTTON_WIDTH = 20;
        private const double PASSWORD_REMOVE_BUTTON_WIDTH = 20;
        #endregion

        #region EncUI
        // Add File
        private void EncFileSelectButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "Open File";
            dialog.Filter = "All Files (*.*)|*.*";
            dialog.Multiselect = true;
            if (dialog.ShowDialog() == true)
            {
                addEncFileNameLbItem(dialog.FileNames);
            }
        }

        // Add Dir
        private void EncDirSelectButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            dialog.Description = "Select Folder";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                addEncFileNameLbItem(new string[]{ dialog.SelectedPath });
            }
        }

        internal void addEncFileNameLbItem(string[] fileName)
        {
            if (EncOutputFileTxt.Text == "")
            {   // set to EncOutputFileTxt
                try
                {
                    string inFile = fileName[0];
                    string path = inFile.Substring(0, inFile.LastIndexOf(@"\"));
                    string file = inFile.Substring(inFile.LastIndexOf(@"\") + 1);
                    if (file.Contains('.'))
                    {
                        file = file.Substring(0, file.IndexOf('.'));
                    }

                    var outputFileName = System.IO.Path.Combine(path, file + ".mpe");
                    EncOutputFileTxt.Text = outputFileName;
                }
                catch (Exception)
                {
                    ;   // Ignored for unexpected character strings.
                    //(J) 想定外文字列の場合は無視
                }
            }

            // add to EncFileNameLb
            foreach (var s in fileName)
            {
                if (EncFileNameLb.Items.Contains(s) == false)
                {
                    EncFileNameLb.Items.Add(s);
                }
            }
        }

        // Remove
        private void EncFileRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (EncFileNameLb.SelectedItems.Count == 0)
            {
                var message = ResourceService.Current.getMessage("MsgIRemoveNoTargetItem")  ;// Excludes the selected file or folder from subject to encryption.
                MessageBox.Show(message, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            while (EncFileNameLb.SelectedItems.Count > 0)
            {
                EncFileNameLb.Items.Remove(EncFileNameLb.SelectedItems[0]);

            }
        }

        // output file select
        private void EncOutputFileSelectButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();

            dialog.Title = "Open File";
            dialog.Filter = "Multiple Passwords Encrypted File (*.mpe)|*.mpe|All Files (*.*)|*.*";

            if (dialog.ShowDialog() == true)
            {
                EncOutputFileTxt.Text = dialog.FileName;
            }
        }

        private List<EncQuestionUI> _encQuestionList = new List<EncQuestionUI>();
        private Dictionary<object, EncQuestionUI> _encQuestionEventDic = new Dictionary<object, EncQuestionUI>();

        // Encryption UI Elements
        public class EncQuestionUI
        {
            public int _index;
            public Label _hintLabel;
            public TextBox _hintTextBox;
            public StackPanel _passwordStackPanel;

            public Button _questionRemoveButton;
            public Button _passwordAddButton;
            public Button _passwordRemoveButton;
        }

        // add question
        private void EncAddQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            var question = new EncQuestionUI();
            _encQuestionList.Add(question);

            int nQuestion = _encQuestionList.Count;
            question._index = nQuestion;

            var border = new Border();
            border.BorderThickness = TH_BOADER;
            border.BorderBrush = Brushes.Gray;
            border.Margin = TH_MARGIN_T5_R5;

            var scrollViewer = new ScrollViewer();
            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            scrollViewer.Padding = TH_PADDING_5;
            border.Child = scrollViewer;

            var questionStackPanel = new StackPanel();
            questionStackPanel.Orientation = Orientation.Vertical;
            scrollViewer.Content = questionStackPanel;

            var hintStackPanel = new StackPanel();
            hintStackPanel.Orientation = Orientation.Horizontal;
            questionStackPanel.Children.Add(hintStackPanel);

            var hintLabel = new Label();
            hintLabel.Content = "Hint" + nQuestion + ":";
            hintLabel.Width = HEADER_WIDTH;
            hintStackPanel.Children.Add(hintLabel);
            question._hintLabel = hintLabel;

            var hintTextBox = new TextBox();
            hintTextBox.MinWidth = HINT_WIDTH;
            hintStackPanel.Children.Add(hintTextBox);
            question._hintTextBox = hintTextBox;

            var questionRemoveButton = new Button();
            questionRemoveButton.Content = "X";
            questionRemoveButton.Width = HINT_REMOVE_BUTTON_WIDTH;
            questionRemoveButton.Margin = TH_MARGIN_L10;
            questionRemoveButton.Click += EncQuestionRemoveButton_Click;
            hintStackPanel.Children.Add(questionRemoveButton);
            question._questionRemoveButton = questionRemoveButton;
            _encQuestionEventDic.Add(questionRemoveButton, question);


            var passwordStackPanel = new StackPanel();
            passwordStackPanel.Orientation = Orientation.Horizontal;
            passwordStackPanel.Margin = TH_MARGIN_T5;
            questionStackPanel.Children.Add(passwordStackPanel);
            question._passwordStackPanel = passwordStackPanel;

            var passwordLabel = new Label();
            passwordLabel.Content = "Password(s):";
            passwordLabel.Width = HEADER_WIDTH;
            passwordStackPanel.Children.Add(passwordLabel);

            var passwordTextBox = new TextBox();
            passwordTextBox.MinWidth = PASSWORD_WIDTH;
            passwordStackPanel.Children.Add(passwordTextBox);

            var passwordAddButton = new Button();
            passwordAddButton.Content = "+";
            passwordAddButton.Width = PASSWORD_ADD_BUTTON_WIDTH;
            passwordAddButton.Margin = TH_MARGIN_L10;
            passwordAddButton.Click += EncPasswordAddButton_Click;
            passwordStackPanel.Children.Add(passwordAddButton);
            question._passwordAddButton = passwordAddButton;
            _encQuestionEventDic.Add(passwordAddButton, question);

            var passwordRemoveButton = new Button();
            passwordRemoveButton.Content = "-";
            passwordRemoveButton.Width = PASSWORD_REMOVE_BUTTON_WIDTH;
            passwordRemoveButton.Click += EncPasswordRemoveButton_Click;
            passwordStackPanel.Children.Add(passwordRemoveButton);
            question._passwordRemoveButton = passwordRemoveButton;
            _encQuestionEventDic.Add(passwordRemoveButton, question);


            EncQuestionListPanel.Children.Insert(nQuestion, border);

            EncNumRequiredPasswordsCmb.Items.Add(nQuestion);
            EncNumRequiredPasswordsCmb.SelectedIndex = (nQuestion - 1);
        }

        private void EncQuestionRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            string message;
            if (_encQuestionList.Count == 1)
            {
                message = ResourceService.Current.getMessage("MsgERemoveLastQuestion"); // Only one Question can not be deleted.
                MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            var question = _encQuestionEventDic[sender];
            message = String.Format(ResourceService.Current.getMessage("MsgCDeleteQuestion"), question._index); // Delete question {0}.
            var result = MessageBox.Show(message, "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation, MessageBoxResult.Cancel);
            if (result == MessageBoxResult.Cancel)
            {
                return;
            }
            EncQuestionListPanel.Children.RemoveAt(question._index);

            _encQuestionList.RemoveAt(question._index - 1);

            for (int i = question._index - 1; i < _encQuestionList.Count; ++i)
            {
                _encQuestionList[i]._index -= 1;
                _encQuestionList[i]._hintLabel.Content = "Hint" + _encQuestionList[i]._index + ":";
            }

            if (EncNumRequiredPasswordsCmb.SelectedIndex >= _encQuestionList.Count)
            {
                EncNumRequiredPasswordsCmb.SelectedIndex = _encQuestionList.Count - 1;
            }
            EncNumRequiredPasswordsCmb.Items.RemoveAt(_encQuestionList.Count);

            _encQuestionEventDic.Remove(question._questionRemoveButton);
            _encQuestionEventDic.Remove(question._passwordAddButton);
            _encQuestionEventDic.Remove(question._passwordRemoveButton);
        }

        private void EncPasswordAddButton_Click(object sender, RoutedEventArgs e)
        {
            var question = _encQuestionEventDic[sender];

            var passwordTextBox = new TextBox();
            passwordTextBox.MinWidth = PASSWORD_WIDTH;
            passwordTextBox.Margin = TH_MARGIN_L10;
            question._passwordStackPanel.Children.Insert(question._passwordStackPanel.Children.Count - 2, passwordTextBox);
            // Last 2 elements are "+" button and "-" button.
        }

        private void EncPasswordRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var question = _encQuestionEventDic[sender];
            UIElementCollection passwords = question._passwordStackPanel.Children;

            bool existsPassword = false;
            // second to last text box.
            for (int i = passwords.Count - 3; i >= 2; --i)
            {
                TextBox passwordTextBox = (TextBox)passwords[i];
                if (Util.isEmpty(passwordTextBox.Text, EncTrimChk.IsChecked))
                {
                    passwords.RemoveAt(i);
                }
                else
                {
                    existsPassword = true;
                }
            }

            // first text box.
            if (existsPassword && Util.isEmpty(((TextBox)passwords[1]).Text, EncTrimChk.IsChecked))
            {
                passwords.RemoveAt(1);
            }

            // The first element is label
        }


        // Encrypt
        private async void EncryptButton_Click(object sender, RoutedEventArgs e)
        {
            DisableUIs();

            // input file check
            string message;
            if (EncFileNameLb.Items.Count == 0)
            {
                message = ResourceService.Current.getMessage("MsgETargetNotSpecified"); // Encryption target file is not specified.
                MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                goto EXIT;
            }
            if (EncOutputFileTxt.Text.Trim() == "")
            {
                message = ResourceService.Current.getMessage("MsgEOutputNotSpecified"); // Output file is not specified.
                MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                goto EXIT;
            }


            var fileList = new List<string>();
            var dirList = new List<string>();
            var notExistFileList = new List<string>();
            var encTargetList = new List<string>();
            foreach (var item in EncFileNameLb.Items)
            {
                string pathFile = item.ToString();
                if (File.Exists(pathFile))
                {
                    fileList.Add(System.IO.Path.GetFileName(pathFile));
                    encTargetList.Add(pathFile);
                }
                else if (Directory.Exists(pathFile))
                {
                    var dir = new DirectoryInfo(pathFile);
                    dirList.Add(dir.Name);
                    encTargetList.Add(pathFile);
                }
                else
                {
                    notExistFileList.Add(pathFile);
                }
            }
            if (fileList.Count != fileList.Distinct().Count() || dirList.Count != dirList.Distinct().Count())
            {
                message = ResourceService.Current.getMessage("MsgETargetDuplicated"); // Encryption is impossible due to duplicate file name or folder name.
                MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                goto EXIT;
            }
            if (notExistFileList.Count > 0)
            {
                message = ResourceService.Current.getMessage("MsgWFileNotExisted"); // The following file does not exist.
                foreach (var s in notExistFileList)
                {
                    message += "\r\n\t" + s;
                }
                if (EncFileNameLb.Items.Count == notExistFileList.Count)
                {
                    message += "\r\n\r\n" + ResourceService.Current.getMessage("MsgETargetNotExisted"); // There is no file to encrypt.
                    MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    goto EXIT;
                }
                else
                {
                    message += "\r\n\r\n" + ResourceService.Current.getMessage("MsgCEncryptOnlyExistFile"); // Encrypt with existing files only?
                    if (MessageBox.Show(message, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No) == MessageBoxResult.No)
                    {
                        goto EXIT;
                    }
                }
            }

            // input password check
            int nQuestions = _encQuestionList.Count;
            int nRequiredPasswords = int.Parse(EncNumRequiredPasswordsCmb.Text);

            if ((bool)EncTrimChk.IsChecked)
            {
                foreach (var question in _encQuestionList)
                {
                    foreach (var passwordText in question._passwordStackPanel.Children.OfType<TextBox>())
                    {
                        passwordText.Text = passwordText.Text.Trim();
                    }
                }
            }

            string[] hintList = new string[nQuestions];
            List<string>[] passwordList = new List<string>[nQuestions];
            int nDummyQuestion = 0;
            for (int i = 0; i < _encQuestionList.Count; ++i)
            {
                var question = _encQuestionList[i];

                hintList[i] = question._hintTextBox.Text;
                passwordList[i] = new List<string>();

                foreach (var passwordText in question._passwordStackPanel.Children.OfType<TextBox>())
                {
                    if (passwordText.Text != "")
                    {
                        passwordList[i].Add(passwordText.Text);
                    }
                }
                if (passwordList[i].Count == 0)
                {
                    nDummyQuestion += 1;
                }
            }

            if (nDummyQuestion > 0)
            {
                message = ResourceService.Current.getMessage("MsgWDummyQuestion"); // A Question without a password is a dummy Question.
                if (nQuestions - nDummyQuestion < nRequiredPasswords)
                {
                    message += "\r\n" + ResourceService.Current.getMessage("MsgWUndecrytableArchive"); // Because it is less than the number of passwords required for decryption, an archive that can not be decrypted is created.
                }
                MessageBox.Show(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK);
            }

            if (File.Exists(EncOutputFileTxt.Text))
            {
                message = ResourceService.Current.getMessage("MsgCOverwriteOutputFile"); // The output file exists. Do you want to overwrite?
                if (MessageBox.Show(message, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No) == MessageBoxResult.No)
                {
                    goto EXIT;
                }
            }

            FileStream outputFS;
            try
            {
                outputFS = File.Create(EncOutputFileTxt.Text);
            }
            catch (Exception ex)
            {
                message = ResourceService.Current.getMessage("MsgEFailCreateFile") + ex.Message; // Failed to create the output file.\r\n\r\nDetailed information:
                MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                goto EXIT;
            }

            message = ResourceService.Current.getMessage("MsgCStartEncryption"); // Start encryption.
            if (MessageBox.Show(message, "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.Cancel) == MessageBoxResult.Cancel)
            {
                outputFS.Close();
                goto EXIT;
            }


            AppConst.MPE_Flag flag = createFlag();
            foreach (var list in passwordList)
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    list[i] = convertPassword(list[i], flag);
                }
            }

            var currentCursor = this.Cursor;
            try
            {
                this.Cursor = Cursors.Wait;
                var p = new Progress<string>(ShowProgress);
                await Task.Run(() => { MPECore.Encrypt(encTargetList, hintList, passwordList, nRequiredPasswords, flag, outputFS, p); });
            }
            catch (Exception ex)
            {
                message = ResourceService.Current.getMessage("MsgEFailEncryption") + ex.Message; // Encryption failed.\r\n\r\nDetailed information:
                MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                goto EXIT;
            }
            finally
            {
                outputFS.Close();
                this.Cursor = currentCursor;
            }
            message = ResourceService.Current.getMessage("MsgIEndEncryption"); // Encryption is complete.
            MessageBox.Show(message, "Succeed", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);

EXIT:
            EnableUIs();
        }

        internal AppConst.MPE_Flag createFlag()
        {
            var flag = AppConst.MPE_Flag.none;
            if ((bool)EncTrimChk.IsChecked)
            {
                flag |= AppConst.MPE_Flag.isTrim;
            }
            if ((bool)EncRemoveSpaceChk.IsChecked)
            {
                flag |= AppConst.MPE_Flag.isRemoveSpace;
            }
            if ((bool)EncIgnoreCaseChk.IsChecked)
            {
                flag |= AppConst.MPE_Flag.isIgnoreCase;
            }
            if ((bool)EncIgnoreZenHanChk.IsChecked)
            {
                flag |= AppConst.MPE_Flag.isIgnoreZenHan;
            }
            if ((bool)EncIgnoreHiraKataChk.IsChecked)
            {
                flag |= AppConst.MPE_Flag.isIgnoreHiraKata;
            }
            if ((bool)EncNoCompressChk.IsChecked)
            {
                flag |= AppConst.MPE_Flag.isNoCompress;
            }
            return  flag;
        }

        internal string   convertPassword(string password, AppConst.MPE_Flag flag)
        {
            if (flag.HasFlag(AppConst.MPE_Flag.isRemoveSpace))
            {
                password = Util.toRemoveInnerSpaces(password);
            }
            if (flag.HasFlag(AppConst.MPE_Flag.isIgnoreCase))
            {
                password = Util.toUpperCase(password);
            }
            if (flag.HasFlag(AppConst.MPE_Flag.isIgnoreZenHan))
            {
                password = Util.toZenkaku(password);
            }
            if (flag.HasFlag(AppConst.MPE_Flag.isIgnoreHiraKata))
            {
                password = Util.toHiragana(password);
            }
            return password;
        }
        #endregion


        #region DecUI
        private void DecFileSelectButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "Open File";
            dialog.Filter = "Multiple Passwords Encrypted File (*.mpe)|*.mpe|All Files (*.*)|*.*";
            if (dialog.ShowDialog() == true)
            {
                setDecEncryptedFileTxt(dialog.FileName);
            }
        }

        private void DecOutputDirSelectButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            dialog.Description = "Select Folder";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DecOutputDirTxt.Text = dialog.SelectedPath;
            }
        }

        internal void setDecEncryptedFileTxt(string fileName)
        {
            if (DecOutputDirTxt.Text == "")
            {
                try
                {
                    string path = fileName.Substring(0, fileName.LastIndexOf(@"\"));
                    DecOutputDirTxt.Text = path;
                }
                catch (Exception)
                {
                    ;   // Ignored for unexpected character strings.
                    //(J) 想定外文字列の場合は無視
                }
            }

            DecEncryptedFileTxt.Text = fileName;
        }

        private void DecEncryptedFileTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            // read MPE File and create Questions UI ekements.
            closeDecMpeFile();
            openDecMpeFile(true);
        }


        // Decrypt
        private async void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            DisableUIs();

            int nQuestions = _decMpeFileInfo._nQuestions;
            string[] passwords = new string[nQuestions];
            for (int i = 0; i < passwords.Length; ++i)
            {
                var passwordTxt = _decPasswordTxts[i];
                if ((bool)DecTrimChk.IsChecked)
                {
                    passwordTxt.Text = passwordTxt.Text.Trim();
                }
                passwords[i] = passwordTxt.Text;
            }

            // output dir
            string outputDir = DecOutputDirTxt.Text;
            if ((bool)DecCreateDirChk.IsChecked)
            {
                string encryptedFilePath = DecEncryptedFileTxt.Text;
                if (encryptedFilePath.Contains(@"\"))
                {
                    encryptedFilePath = encryptedFilePath.Substring(encryptedFilePath.LastIndexOf(@"\") + 1);
                }
                if (encryptedFilePath.Contains("."))
                {
                    encryptedFilePath = encryptedFilePath.Substring(0, encryptedFilePath.IndexOf('.'));
                }
                outputDir = System.IO.Path.Combine(outputDir, encryptedFilePath);
            }

            for (int i = 0; i < passwords.Length; ++i)
            {
                passwords[i] = convertPassword(passwords[i], (AppConst.MPE_Flag)_decMpeFileInfo._flag);
            }

            var currentCursor = this.Cursor;
            try
            {
                this.Cursor = Cursors.Wait;
                var p = new Progress<string>(ShowProgress);
                bool result = await Task<bool>.Run(() => { return MPECore.Decrypt(_decMpeFileInfo, passwords, outputDir, p); });
                this.Cursor = currentCursor;
                if (result)
                {
                    var message = ResourceService.Current.getMessage("MsgIEndDecryption"); // Decryption is complete.
                    MessageBox.Show(message, "Succeed", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                }
                else
                {
                    var message = ResourceService.Current.getMessage("MsgIPasswordIncorrect"); // Password is incorrect.
                    MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK);
                }
            }
            catch (Exception ex)
            {
                var message = "Error. detail:" + ex.Message;
                MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                this.Cursor = currentCursor;
            }

            EnableUIs();
        }

        private void DecDecompressDesktopChk_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)DecDecompressDesktopChk.IsChecked)
            {
                DecOutputDirTxt.IsEnabled = false;
                DecOutputDirSelectBtn.IsEnabled = false;

                DecOutputDirTxt.Text = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            }
            else
            {
                DecOutputDirTxt.IsEnabled = true;
                DecOutputDirSelectBtn.IsEnabled = true;

                string encryptedFile = DecEncryptedFileTxt.Text;
                string path;
                if (encryptedFile != "" && encryptedFile.LastIndexOf(@"\") >= 0)
                {
                    path = encryptedFile.Substring(0, encryptedFile.LastIndexOf(@"\"));
                }
                else
                {
                    path = "";
                }
                DecOutputDirTxt.Text = path;
            }
        }


        // MPE File Info
        private DecMPEFileInfo _decMpeFileInfo = null;
        private TextBox[] _decPasswordTxts;

        internal void closeDecMpeFile()
        {
            if (_decMpeFileInfo != null)
            {
                DecryptBtn.IsEnabled = false;
                DecQuestionListPanel.Children.RemoveRange(1, DecQuestionListPanel.Children.Count - 1);

                _decMpeFileInfo._encryptedFS.Close();
                _decMpeFileInfo = null;
            }
        }

        internal void openDecMpeFile(bool isShowMessage)
        {
            if (File.Exists(DecEncryptedFileTxt.Text) == false)
            {
                return;
            }
            var currentCursor = this.Cursor;
            this.Cursor = Cursors.Wait;

            string message;
            var decMPEfileInfo = MPECore.readHeader(DecEncryptedFileTxt.Text);
            if (decMPEfileInfo._errorDetailMessage != "")
            {
                if (isShowMessage)
                {
                    message = ResourceService.Current.getMessage("MsgETargetNotMPE") + decMPEfileInfo._errorDetailMessage; // The specified file is not a Multiple Passwords Encrypted file.\r\n\r\nDetailed information:
                    MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                }
                goto EXIT;
            }
            int nQuestions = decMPEfileInfo._nQuestions;
            int nRequiredPasswords = decMPEfileInfo._nRequiredPasswords;

            // create UI
            _decPasswordTxts = new TextBox[nQuestions];
            setFlag(decMPEfileInfo._flag);

            DecryptBtn.IsEnabled = true;
            DecNumRequiredPasswordsCmb.Items.Clear();
            DecNumRequiredPasswordsCmb.Items.Add(nRequiredPasswords);
            DecNumRequiredPasswordsCmb.SelectedIndex = 0;
            for (int i = 0; i < nQuestions; ++i)
            {
                var question = decMPEfileInfo._decQuestions[i];

                var border = new Border();
                border.BorderThickness = TH_BOADER;
                border.BorderBrush = Brushes.Gray;
                border.Margin = TH_MARGIN_T5_R5;

                var scrollViewer = new ScrollViewer();
                scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                scrollViewer.Padding = TH_PADDING_5;
                border.Child = scrollViewer;

                var questionStackPanel = new StackPanel();
                questionStackPanel.Orientation = Orientation.Vertical;
                scrollViewer.Content = questionStackPanel;

                var hintStackPanel = new StackPanel();
                hintStackPanel.Orientation = Orientation.Horizontal;
                questionStackPanel.Children.Add(hintStackPanel);

                var hintLabel = new Label();
                hintLabel.Content = "Hint" + (i + 1) + ":";
                hintLabel.Width = HEADER_WIDTH;
                hintStackPanel.Children.Add(hintLabel);

                var hintTextBox = new TextBox();
                hintTextBox.MinWidth = HINT_WIDTH;
                hintTextBox.IsEnabled = false;
                hintStackPanel.Children.Add(hintTextBox);
                hintTextBox.Text = question.hint;


                var questionRemoveButton = new Button();
                questionRemoveButton.Content = "X";
                questionRemoveButton.Width = HINT_REMOVE_BUTTON_WIDTH;
                questionRemoveButton.Margin = TH_MARGIN_L10;
                questionRemoveButton.IsEnabled = false;
                hintStackPanel.Children.Add(questionRemoveButton);


                var passwordStackPanel = new StackPanel();
                passwordStackPanel.Orientation = Orientation.Horizontal;
                passwordStackPanel.Margin = TH_MARGIN_T5;
                questionStackPanel.Children.Add(passwordStackPanel);

                var passwordLabel = new Label();
                passwordLabel.Content = "Password:";
                passwordLabel.Width = HEADER_WIDTH;
                passwordStackPanel.Children.Add(passwordLabel);

                var passwordTextBox = new TextBox();
                passwordTextBox.MinWidth = PASSWORD_WIDTH;
                passwordStackPanel.Children.Add(passwordTextBox);
                _decPasswordTxts[i] = passwordTextBox;

                DecQuestionListPanel.Children.Add(border);
            }

            if (decMPEfileInfo._warningDetailMessage != "" && isShowMessage)
            {
                message = ResourceService.Current.getMessage("MsgWMPEInvalidFormat") + decMPEfileInfo._warningDetailMessage; // The specified file may not be decrypted properly.\r\n\r\nDetailed information:
                MessageBox.Show(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
            }

            _decMpeFileInfo = decMPEfileInfo;
EXIT:
            this.Cursor = currentCursor;
        }

        internal void setFlag(int flagValue)
        {
            var flag = (AppConst.MPE_Flag)flagValue;

            DecTrimChk.IsChecked = flag.HasFlag(AppConst.MPE_Flag.isTrim);
            DecRemoveSpaceChk.IsChecked = flag.HasFlag(AppConst.MPE_Flag.isRemoveSpace);
            DecIgnoreCaseChk.IsChecked = flag.HasFlag(AppConst.MPE_Flag.isIgnoreCase);
            DecIgnoreZenHanChk.IsChecked = flag.HasFlag(AppConst.MPE_Flag.isIgnoreZenHan);
            DecIgnoreHiraKataChk.IsChecked = flag.HasFlag(AppConst.MPE_Flag.isIgnoreHiraKata);
            DecNoCompressChk.IsChecked = flag.HasFlag(AppConst.MPE_Flag.isNoCompress);
        }
        #endregion


        #region Window Events
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EncTab.IsSelected)
            {
                closeDecMpeFile();
            }
            else
            {
                if (e.OriginalSource != DecNumRequiredPasswordsCmb)
                {
                    // TabControl_SelectionChanged () seems to be called not only when switching between tabs, but also when changing the internal combo box selection.
                    // Ignore the events that occur at that time to avoid duplicate file open processing.
                    //(J) TabControl_SelectionChanged() は、タブの切り替えだけでなく、内部のコンボボックスの選択変更時でも呼び出される模様。
                    //(J) 二重でファイルオープン処理が行われるのを避けるため、その際に発生するイベントを無視する。
                    openDecMpeFile(false);
                }
            }
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files != null)
            {
                if (EncTab.IsSelected)
                {
                    addEncFileNameLbItem(files);
                }
                else
                {
                    setDecEncryptedFileTxt(files[0]);
                }
            }
        }

        private void Window_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
                e.Effects = DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;
            e.Handled = true;
        }


        private void MenuLang_Click(object sender, RoutedEventArgs e)
        {
            string lang;
            if (sender == MenuLangJa)
            {
                MenuLangEn.IsChecked = false;
                MenuLangJa.IsChecked = true;
                lang = "ja-JP";
            }
            else
            {
                MenuLangEn.IsChecked = true;
                MenuLangJa.IsChecked = false;
                lang = "en-US";
            }
            ResourceService.Current.ChangeCulture(lang);
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuVersion_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(AppConst.VERSION_STR, "Version");
        }
        #endregion

        #region UI Util
        private void DisableUIs()
        {
            EncTab.IsEnabled = false;
            EncryptBtn.IsEnabled = false;

            DecTab.IsEnabled = false;
            DecryptBtn.IsEnabled = false;
        }

        private void EnableUIs()
        {
            EncTab.IsEnabled = true;
            EncryptBtn.IsEnabled = true;

            DecTab.IsEnabled = true;
            DecryptBtn.IsEnabled = true;
        }

        private void ShowProgress(string message)
        {
            StatusBarLabel.Content = message;
        }

        #endregion
    }
}
