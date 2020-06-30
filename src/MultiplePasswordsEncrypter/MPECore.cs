using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MultiplePasswordsEncrypter
{
    // Core logics for Multiple Passwords Encryption.
    class MPECore
    {
        /**
         * First zip the file and then encrypt it.
         * (J) 最初にファイルをzip圧縮し、その後に暗号化する。
         * 
         * encTargetList : Files or directories for encryption.
         * hints : question's hint 
         * passwordLists : password List for hint.
         * nRequiredPasswords :  number of passwords are required for decryption.
         * flag : Encryption flag
         * outputFS : output file stream.
         */
        internal static void Encrypt(
            List<string> encTargetList, 
            string[] hints, 
            List<string>[] passwordLists, 
            int nRequiredPasswords, 
            AppConst.MPE_Flag flag, 
            FileStream outputFS,
            IProgress<string> progress = null)
        {
            var tmpFileCompress = System.IO.Path.GetRandomFileName();   // temporary file name for zip file.
            try
            {
                progress?.Report("(1/4) creating archive.");
                // zip
                CompressionLevel compressionLevel = flag.HasFlag(AppConst.MPE_Flag.isNoCompress) ? CompressionLevel.NoCompression : CompressionLevel.Optimal;
                using (FileStream zipFS = File.Create(tmpFileCompress))
                {
                    using (ZipArchive archive = new ZipArchive(zipFS, ZipArchiveMode.Create))
                    {
                        foreach (var item in encTargetList)
                        {
                            string rootPathFile = item.ToString();
                            if (File.Exists(rootPathFile))
                            {   // Add the file as it is.
                                //(J) ファイルはそのまま追加
                                archive.CreateEntryFromFile(rootPathFile, System.IO.Path.GetFileName(rootPathFile), compressionLevel);
                            }
                            else if (Directory.Exists(rootPathFile))
                            {   // Add directory recursively
                                //(J) ディレクトリは再帰的に追加
                                string parentPath = System.IO.Directory.GetParent(rootPathFile).ToString();
                                string[] pathFiles = Directory.GetFileSystemEntries(rootPathFile, "*", System.IO.SearchOption.AllDirectories);
                                if (pathFiles.Length == 0)
                                {   // Empty directory added only entry.
                                    //(J) 空ディレクトリはエントリのみ追加
                                    archive.CreateEntry(System.IO.Path.GetFullPath(rootPathFile).Substring(parentPath.Length + 1) + "/");
                                }
                                foreach (var pathFile in pathFiles)
                                {
                                    string entryPath = pathFile.Substring(parentPath.Length + 1);   // Archive the specified directory as root. (J)指定したディレクトリをルートとしてアーカイブ
                                    if (File.Exists(pathFile))
                                    {
                                        archive.CreateEntryFromFile(pathFile, entryPath, compressionLevel);
                                    }
                                    else if (Directory.Exists(pathFile))
                                    {
                                        archive.CreateEntry(entryPath + "/");
                                    }
                                }
                            }
                        }
                    }
                }

                // File is not properly terminated without Dispose().
                // Dispose() archive, zipFS will also be closed(), so open it again.
                //(J) 一旦 archive を Dispose() しないと正しくファイルが終端化されない。
                //(J) archive を Dispose() すると zipFS も Close()されてしまうため、開きなおす。

                progress?.Report("(2/4) encrypting archive.");
                var tmpFileEncrypted = System.IO.Path.GetRandomFileName();  // temporary file name for encrypted file.
                try
                {
                    // encryption
                    RNGCryptoServiceProvider r = new RNGCryptoServiceProvider();
                    byte[] dataKey = new byte[Crypto.AES_KEY_SIZE_BYTE];    // Encryption key for data.
                    r.GetBytes(dataKey);

                    using (FileStream zipFS = File.Open(tmpFileCompress, FileMode.Open))
                    {
                        using (FileStream encryptedFS = File.Create(tmpFileEncrypted))
                        {
                            Crypto.encryptStream(zipFS, encryptedFS, dataKey);
                        }
                    }
                    // Since it became unnecessary, try to delete here
                    //(J) 不要になったのでここで削除を試みる
                    File.Delete(tmpFileCompress);                      

                    using (FileStream encryptedFS = File.Open(tmpFileEncrypted, FileMode.Open))
                    {
                        Encrypt_2(encryptedFS, hints, passwordLists, nRequiredPasswords, flag, r, dataKey, outputFS, progress);
                    }
                    progress?.Report("(4/4) finished.");
                }
                finally
                {
                    File.Delete(tmpFileEncrypted);
                }
            }
            finally
            {
                File.Delete(tmpFileCompress);
            }
        }

        /**
         * encryption
         */
        internal static void Encrypt_2(
            FileStream encryptedFS, 
            string[] hintList, 
            List<string>[] passwordList, 
            int nRequiredPasswords, 
            AppConst.MPE_Flag flag, 
            RNGCryptoServiceProvider r, 
            byte[] dataKey, 
            FileStream outputFS,
            IProgress<string> progress = null)
        {
            // generate questionKeys, encryptedQuestionKeys
            int nQuestions = hintList.Length;
            byte[][] questionKeys = new byte[nQuestions][];
            byte[][][] encryptedQuestionKeys = new byte[nQuestions][][];
            for (int i = 0; i < nQuestions; ++i)
            {
                questionKeys[i] = new byte[Crypto.AES_KEY_SIZE_BYTE];   // Encryption key for dataKey
                r.GetBytes(questionKeys[i]);

                // encrypt questionKey
                // Based on the number of passwords, assign the location of the password to encrypt questionKeys. In order to hide the number of passwords, dummy indexes are also included.
                // For example, when three passwords are set in the Question being processed, the following values ​​are randomly set in passwordEncryptIndexes.
                // passwordEncryptIndexes = {1, -1, 0, 2, -1}
                // In this case, encryptedQuestionKeys is [question 0] encrypted questionKey with password 1, [1]: dummy, [2]: questionKey encrypted with password 0, [3]: questionKey encrypted with password 2 , [4]: ​​Dummy is stored.
                //(J) パスワードの個数をもとに、questionKeysを暗号化するパスワードの位置を割り当てる。パスワード個数を隠蔽するため、ダミーのインデクスも含まれる。
                //(J) たとえば、処理中の Question にパスワードが3つ設定されている場合に、passwordEncryptIndexes には以下のような値をランダムに設定する。
                //(J) passwordEncryptIndexes = {1, -1, 0, 2, -1}
                //(J) この場合はencryptedQuestionKeys は、[0]:パスワード1で暗号化されたquestionKey, [1]:ダミー, [2]:パスワード0で暗号化されたquestionKey, [3]:パスワード2で暗号化されたquestionKey, [4]:ダミー が格納される。
                int[] passwordEncryptIndexes = alllocPasswordEncryptIndexes(passwordList[i].Count);
                encryptedQuestionKeys[i] = new byte[passwordEncryptIndexes.Length][];
                for (int j = 0; j < passwordEncryptIndexes.Length; ++j)
                {
                    if (passwordEncryptIndexes[j] >= 0)
                    {   // real password                   
                        String password = passwordList[i][passwordEncryptIndexes[j]];
                        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                        encryptedQuestionKeys[i][j] = Crypto.encryptByte(questionKeys[i], passwordBytes, Crypto.createSalt(i, j));
                        if (encryptedQuestionKeys[i][j].Length != Crypto.AES_KEY_SIZE_BYTE_WITH_PADDING)
                        {   // assertion
                            throw new Exception("system error");    //!
                        }
                    }
                    else
                    {   // dummy key
                        byte[] dummyKey = new byte[Crypto.AES_KEY_SIZE_BYTE_WITH_PADDING];
                        r.GetBytes(dummyKey);
                        encryptedQuestionKeys[i][j] = dummyKey;
                    }
                }
            }

            // generate encryptedDataKey
            int[][] combinaiton = Combination.enumerateCombination(nQuestions, nRequiredPasswords);
            byte[] combinedQuestionKey = new byte[nRequiredPasswords * Crypto.AES_KEY_SIZE_BYTE];
            byte[][] encryptedDataKey = new byte[combinaiton.Length][];
            for (int i = 0; i < combinaiton.Length; ++i)
            {
                // Create a password for dataKey from multiple question keys.
                //(J) 複数のquestion key から dataKey 用のパスワードを作成する
                for (int j = 0; j < combinaiton[i].Length; ++j)
                {
                    int qkIdx = combinaiton[i][j];
                    Buffer.BlockCopy(questionKeys[qkIdx], 0, combinedQuestionKey, j * Crypto.AES_KEY_SIZE_BYTE, Crypto.AES_KEY_SIZE_BYTE);
                }

                encryptedDataKey[i] = Crypto.encryptByte(dataKey, combinedQuestionKey);
                if (encryptedDataKey[i].Length != Crypto.AES_KEY_SIZE_BYTE_WITH_PADDING)
                {   // assertion
                    throw new Exception("system error");    //!
                }
            }

            progress?.Report("(3/4) creating output file.");
            // create question Header (for MPE File)
            using (var questionHeaderMS = new MemoryStream())
            {
                byte[] bytes;
                //	flag				1
                questionHeaderMS.WriteByte((byte)flag);
                //	nQuestions			4
                bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(nQuestions));
                questionHeaderMS.Write(bytes, 0, bytes.Length);
                //	nRequiredPasswords		4
                bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(nRequiredPasswords));
                questionHeaderMS.Write(bytes, 0, bytes.Length);
                //	nPasswordCombinations	4
                bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(encryptedDataKey.Length));
                questionHeaderMS.Write(bytes, 0, bytes.Length);
                //	encryptedDataKeys	n  * 48
                for (int i = 0; i < encryptedDataKey.Length; ++i)
                {
                    questionHeaderMS.Write(encryptedDataKey[i], 0, encryptedDataKey[i].Length);
                }

                //	Question			n
                for (int i = 0; i < nQuestions; ++i)
                {
                    // hint string len      4
                    byte[] hintStrBytes = Encoding.UTF8.GetBytes(hintList[i]);
                    bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(hintStrBytes.Length));
                    questionHeaderMS.Write(bytes, 0, bytes.Length);
                    // hint string          n
                    questionHeaderMS.Write(hintStrBytes, 0, hintStrBytes.Length);
                    // nPasswords            4
                    bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(encryptedQuestionKeys[i].Length));
                    questionHeaderMS.Write(bytes, 0, bytes.Length);
                    // encryptedQuestionKeys n * 48
                    for (int j = 0; j < encryptedQuestionKeys[i].Length; ++j)
                    {
                        questionHeaderMS.Write(encryptedQuestionKeys[i][j], 0, encryptedQuestionKeys[i][j].Length);
                    }
                }

                Encrypt_3(questionHeaderMS, encryptedFS, outputFS);
            }
        }

        private const int MIN_N_KEYS_WITH_DUMMY = 4;
        /**
         * Based on the number of passwords, assign the location of the password to encrypt questionKeys. In order to hide the number of passwords, dummy indexes are also included.
         * (J)パスワードの個数をもとに、questionKeysを暗号化するパスワードの位置を割り当てる。パスワード個数を隠蔽するため、ダミーのインデクスも含まれる。
         */
        private static int[] alllocPasswordEncryptIndexes(int n)
        {
            var r = new Random();

            int nDummyIndexes = r.Next(7);  // add +0 to +7 dummy data
            while (n + nDummyIndexes <= MIN_N_KEYS_WITH_DUMMY)
            {   // at least 4 passwords 
                nDummyIndexes += r.Next(7);  // add +0 to +7 dummy data
            }

            var indexList = new List<int>();
            for (int i = 0; i < n; ++i)
            {
                indexList.Add(i);
            }
            for (int i = 0; i < nDummyIndexes; ++i)
            {
                indexList.Add(-1);
            }

            return indexList.OrderBy(i => Guid.NewGuid()).ToArray();    // shuffle
        }

        /**
         * calculate CRC, write header, Question header, data.
         */
        internal static void Encrypt_3(MemoryStream questionHeaderMS, FileStream encryptedFS, FileStream outputFS)
        {
            byte[] questionHeaderCRC;
            using (CRC crcProvider = new CRC(CRCpolynomial.CRC32_C))
            {
                questionHeaderMS.Position = 0;
                questionHeaderCRC = crcProvider.ComputeHash(questionHeaderMS);
            }

            byte[] dataCRC;
            using (CRC crcProvider = new CRC(CRCpolynomial.CRC32_C))
            {
                encryptedFS.Position = 0;
                dataCRC = crcProvider.ComputeHash(encryptedFS);
            }

            string header = AppConst.MPE_HEADER_CONST + AppConst.MPE_FORMAT_VERSION + AppConst.MPE_ENCODER_VERSION;
            byte[] ba = Encoding.UTF8.GetBytes(header);
            outputFS.Write(ba, 0, ba.Length);

            ba = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)questionHeaderMS.Length));
            outputFS.Write(ba, 0, ba.Length);
            outputFS.Write(questionHeaderCRC, 0, questionHeaderCRC.Length);

            ba = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(encryptedFS.Length));
            outputFS.Write(ba, 0, ba.Length);
            outputFS.Write(dataCRC, 0, dataCRC.Length);

            questionHeaderMS.Position = 0;
            questionHeaderMS.CopyTo(outputFS);
            encryptedFS.Position = 0;
            encryptedFS.CopyTo(outputFS);
        }


        // MPE file info for deryption.
        public class DecMPEFileInfo
        {
            public FileStream _encryptedFS;
            public long _dataStartPosition;
            public byte _flag;
            public int _nQuestions;
            public int _nRequiredPasswords;
            public byte[] _encryptedDataKeys;
            public DecQuestion[] _decQuestions;

            public string _errorDetailMessage;
            public string _warningDetailMessage;
        }

        // Question info int MPE File
        public class DecQuestion
        {
            public string hint;
            public int nPasswords;
            public byte[] encryptedQuestionKeys;
        }

        internal static DecMPEFileInfo readHeader(string encryptedFileName)
        {
            // Anaryze MPE File.
            //(J) MPEファイル解析
            string errorDetailMessage = "";
            string warningDetailMessage = "";
            var encryptedFS = new FileStream(encryptedFileName, FileMode.Open);

            byte[] buf = new byte[AppConst.MPE_HEADER_SIZE];
            int readByte = encryptedFS.Read(buf, 0, AppConst.MPE_HEADER_SIZE);
            if (readByte < AppConst.MPE_HEADER_SIZE)
            {
                errorDetailMessage = "File size is too small.";
                goto ERROR;
            }

            var reader = new MPEHeaderReader(buf);

            string headerConst = reader.ReadString(3);
            if (headerConst != AppConst.MPE_HEADER_CONST)
            {
                errorDetailMessage = "MPE Header constant is incorrect.";
                goto ERROR;
            }

            string warningDetail = "";
            string archiveVersion = reader.ReadString(3);
            if (archiveVersion != AppConst.MPE_FORMAT_VERSION)
            {
                warningDetail += "archive format version:" + archiveVersion + " preferred:000\r\n";
            }
            string encoderVersion = reader.ReadString(3);
            if (encoderVersion != AppConst.MPE_ENCODER_VERSION)
            {
                warningDetail += "encoder version:" + encoderVersion + " preferred:000\r\n";
            }

            int questionHeaderLen = reader.ReadInt();
            byte[] questionHeaderCRC = reader.ReadBytes(4);
            long dataLen = reader.ReadLong();
            byte[] dataCRC = reader.ReadBytes(4);

            if (AppConst.MPE_HEADER_SIZE + questionHeaderLen + dataLen != encryptedFS.Length)
            {
                errorDetailMessage = "File size is incorrect.";
                goto ERROR;
            }

            // question header 
            byte[] questionHeaderBuf = new byte[questionHeaderLen];
            encryptedFS.Read(questionHeaderBuf, 0, questionHeaderLen);
            byte[] questionHeaderCRCVerify;
            using (CRC crcProvider = new CRC(CRCpolynomial.CRC32_C))
            {
                questionHeaderCRCVerify = crcProvider.ComputeHash(questionHeaderBuf);
            }
            if (questionHeaderCRC.SequenceEqual(questionHeaderCRCVerify) == false)
            {
                warningDetail += "question header CRC32 unmatched.\r\n";
            }

            // TODO:別スレッドで検算
            long dataStartPosition = encryptedFS.Position;
            byte[] dataCRCVerify;
            using (CRC crcProvider = new CRC(CRCpolynomial.CRC32_C))
            {
                dataCRCVerify = crcProvider.ComputeHash(encryptedFS);
            }
            if (dataCRC.SequenceEqual(dataCRCVerify) == false)
            {
                warningDetail += "data CRC32 unmatched.\r\n";
            }

            byte flag;
            int nQuestions;
            int nRequiredPasswords;
            int nPasswordCombinations;
            byte[] encryptedDataKeys;
            DecQuestion[] questions;
            try
            {
                // parse question header 
                reader = new MPEHeaderReader(questionHeaderBuf);

                flag = reader.ReadByte();
                nQuestions = reader.ReadInt();
                nRequiredPasswords = reader.ReadInt();
                nPasswordCombinations = reader.ReadInt();
                // assertion: nQuestions C nRequiredPasswords == nPasswordCombinations
                encryptedDataKeys = reader.ReadBytes(nPasswordCombinations * Crypto.AES_KEY_SIZE_BYTE_WITH_PADDING);

                questions = new DecQuestion[nQuestions];
                for (int i = 0; i < nQuestions; ++i)
                {
                    var question = new DecQuestion();
                    int hintLen = reader.ReadInt();
                    question.hint = reader.ReadString(hintLen);
                    question.nPasswords = reader.ReadInt();
                    question.encryptedQuestionKeys = reader.ReadBytes(question.nPasswords * Crypto.AES_KEY_SIZE_BYTE_WITH_PADDING);

                    questions[i] = question;
                }

                if (reader.Position != questionHeaderBuf.Length)
                {
                    errorDetailMessage = "MPE Header is incorrect.";
                    goto ERROR;
                }
            }
            catch (IndexOutOfRangeException)
            {
                errorDetailMessage = "MPE Header is incorrect.";
                goto ERROR;
            }

            var decMpeFileInfo = new DecMPEFileInfo();
            decMpeFileInfo._encryptedFS = encryptedFS;
            decMpeFileInfo._dataStartPosition = dataStartPosition;
            decMpeFileInfo._flag = flag;
            decMpeFileInfo._nQuestions = nQuestions;
            decMpeFileInfo._nRequiredPasswords = nRequiredPasswords;
            decMpeFileInfo._encryptedDataKeys = encryptedDataKeys;
            decMpeFileInfo._decQuestions = questions;

            decMpeFileInfo._errorDetailMessage = errorDetailMessage;
            decMpeFileInfo._warningDetailMessage = warningDetailMessage;
            return decMpeFileInfo;

        ERROR:
            encryptedFS.Close();
            decMpeFileInfo = new DecMPEFileInfo();
            decMpeFileInfo._warningDetailMessage = warningDetailMessage;
            decMpeFileInfo._errorDetailMessage = errorDetailMessage;
            return decMpeFileInfo;
        }

        internal static bool   Decrypt(
            DecMPEFileInfo decMpeFileInfo, 
            string[] passwords, 
            string outputDir,
            IProgress<string> progress = null)
        {
            int nQuestions = decMpeFileInfo._nQuestions;
            int nRequiredPasswords = decMpeFileInfo._nRequiredPasswords;

            int nDecryptedQuestionKeys = 0; // Number of QuestionKeys that could be decoded. (J)複号できたQuestionKeyの数
            int[] questionKeyIndexes = new int[nRequiredPasswords]; // Index of QuestionKeys that could be decoded. (J)複号できたQuestionKeyのIndex
            byte[] questionKeys = new byte[nRequiredPasswords * Crypto.AES_KEY_SIZE_BYTE];  // Combined Question Key (for decrypt dataKey)

            // check password
            for (int i = 0; i < nQuestions && nDecryptedQuestionKeys < nRequiredPasswords; ++i)
            {
                var passwordTxt = passwords[i];
                if (passwordTxt == "")
                {
                    continue;
                }
                var question = decMpeFileInfo._decQuestions[i];

                byte[] password = Encoding.UTF8.GetBytes(passwordTxt);
                for (int j = 0; j < question.nPasswords; ++j)
                {
                    var result = Crypto.decryptByte(question.encryptedQuestionKeys, j * Crypto.AES_KEY_SIZE_BYTE_WITH_PADDING, Crypto.AES_KEY_SIZE_BYTE_WITH_PADDING, password, Crypto.createSalt(i, j));
                    if (result != null)
                    {
                        if (result.Length != Crypto.AES_KEY_SIZE_BYTE)
                        {   // assertion
                            throw new Exception("broken file");
                        }

                        // store question key
                        Buffer.BlockCopy(result, 0, questionKeys, nDecryptedQuestionKeys * Crypto.AES_KEY_SIZE_BYTE, Crypto.AES_KEY_SIZE_BYTE);
                        questionKeyIndexes[nDecryptedQuestionKeys] = i;
                        nDecryptedQuestionKeys += 1;
                        break;
                    }
                }
            }

            if (nDecryptedQuestionKeys < nRequiredPasswords)
            {
                return false;   // fail. number of passwords is not enough.
            }


            // decrypt data
            byte[] dataKey = null;
            int[][] combinaitons = Combination.enumerateCombination(nQuestions, nRequiredPasswords);
            for (int i = 0; i < combinaitons.Length; ++i)
            {
                if (combinaitons[i].SequenceEqual(questionKeyIndexes))
                {
                    dataKey = Crypto.decryptByte(decMpeFileInfo._encryptedDataKeys, i * Crypto.AES_KEY_SIZE_BYTE_WITH_PADDING, Crypto.AES_KEY_SIZE_BYTE_WITH_PADDING, questionKeys);
                    break;
                }
            }

            if (dataKey == null)
            {
                throw new Exception("broken file");
            }

            progress?.Report("(1/3) decripting archive.");
            // OK
            var tmpFileCompress = System.IO.Path.GetRandomFileName();
            try
            {
                using (FileStream zipFS = new FileStream(tmpFileCompress, FileMode.Create))
                {
                    decMpeFileInfo._encryptedFS.Position = decMpeFileInfo._dataStartPosition;
                    Crypto.decryptStream(decMpeFileInfo._encryptedFS, zipFS, dataKey);
                }

                // ZipFile.ExtractToDirectory can not read files unless Close() is done on zipFS
                //(J) zipFS を Close() しないと ZipFile.ExtractToDirectory はファイルを読み込めない
                try
                {
                    progress?.Report("(2/3) creating output file(s).");
                    System.IO.Compression.ZipFile.ExtractToDirectory(tmpFileCompress, outputDir);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            finally
            {
                File.Delete(tmpFileCompress);
            }
            progress?.Report("(3/3) finished.");

            return true;    // succeed
        }
    }
}
