using MultiplePasswordsEncrypter.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MultiplePasswordsEncrypter
{
    // Multilingual Module
    // author:
    // https://qiita.com/YSRKEN/items/a96bcec8dfb0a8340a5f
    public class ResourceService : INotifyPropertyChanged
    {
        #region シングルトン対策
        private static readonly ResourceService current = new ResourceService();
        public static ResourceService Current => current;
        #endregion

        #region INotifyPropertyChanged対策
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        /// <summary>
        /// リソースを取得
        /// </summary>
        private readonly Resources resources = new Resources();
        public Resources Resources => resources;

        public CultureInfo CultureInfo;

        /// <summary>
        /// リソースのカルチャーを変更
        /// </summary>
        /// <param name="name">カルチャー名</param>
        public void ChangeCulture(string name)
        {
            CultureInfo = CultureInfo.GetCultureInfo(name);
            Resources.Culture = CultureInfo;
            RaisePropertyChanged("Resources");
        }

        public string getMessage(string messageKey)
        {
            return Properties.Resources.ResourceManager.GetString(messageKey, this.CultureInfo).Replace("\\n", "\n");
        }
    }
}
