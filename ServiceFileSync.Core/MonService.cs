using System;
using System.IO;

namespace ServiceFileSync.Core
{
    public class MonService
    {
        #region Fields
        /// <summary>
        ///     Chemin du dossier d'entrée
        /// </summary>
        private string _FirstFolderPath;

        /// <summary>
        ///     Chemin du dossier de sortie
        /// </summary>
        private string _SecondFolderPath;

        /// <summary>
        ///     Classe d'écoute du premier répertoire
        /// </summary>
        private FileSystemWatcher _FirstWatcher;

        /// <summary>
        ///     Classe d'écoute du second répertoire
        /// </summary>
        private FileSystemWatcher _SecondWatcher;
        #endregion

        #region Constructors
        /// <summary>
        ///     Initialise une nouvelle instance de la classe <see cref="MonService"/>.
        /// </summary>
        /// <param name="firstFolderPath">Chemin du premier répertoire.</param>
        /// <param name="secondFolderPath">Chemin du second répertoire.</param>
        public MonService(string firstFolderPath, string secondFolderPath)
        {
            this._FirstFolderPath = !string.IsNullOrWhiteSpace(firstFolderPath) ?
                                        firstFolderPath : throw new ArgumentNullException(nameof(firstFolderPath));
            this._SecondFolderPath = !string.IsNullOrWhiteSpace(secondFolderPath) ?
                                        secondFolderPath : throw new ArgumentNullException(nameof(secondFolderPath));
        }
        #endregion

        #region Methods

        #region Service

        /// <summary>
        ///     Démarre le service.
        /// </summary>
        public void Start()
        {
            if (this._FirstWatcher == null)
            {
                try
                {
                    //Créer le premier dossier s'il n'existe pas.
                    Directory.CreateDirectory(this._FirstFolderPath);
                    //Créer le second dossier s'il n'existe pas.
                    Directory.CreateDirectory(this._SecondFolderPath);
                }
                catch (Exception ex)
                {
                    throw new Exception("Impossible de créer le dossier d'entrée ou de sortie", ex);
                }

                this._FirstWatcher = new FileSystemWatcher(this._FirstFolderPath);
                this._FirstWatcher.Created += this.OneWay;
                this._FirstWatcher.EnableRaisingEvents = true;
            }
        }

        /// <summary>
        ///     Met en pause l'exécution du service.
        /// </summary>
        public void Pause()
        {
            if (this._FirstWatcher != null && this._FirstWatcher.EnableRaisingEvents)
            {
                this._FirstWatcher.EnableRaisingEvents = false;
            }
        }

        /// <summary>
        ///     Reprend l'exécution du service.
        /// </summary>
        public void Continue()
        {
            if (this._FirstWatcher != null && !this._FirstWatcher.EnableRaisingEvents)
            {
                this._FirstWatcher.EnableRaisingEvents = true;
            }
        }

        /// <summary>
        ///     Arrête l'exécution du service.
        /// </summary>
        public void Stop()
        {
            if (this._FirstWatcher != null)
            {
                //this._FirstWatcher.Created -= this.Watcher_Created;
                this._FirstWatcher.Dispose();
                this._FirstWatcher = null;
            }
        }

        #endregion

        private void OneWay(object sender, FileSystemEventArgs e)
        {
            try
            {
                //Clear second folder
                if (Directory.Exists(this._SecondFolderPath))
                {
                    //Remove the existing files
                    string[] files = Directory.GetFiles(this._SecondFolderPath);

                    foreach (string file in files)
                    {
                        File.Delete(file);
                    }

                    //Remove the existing directories
                    string[] directories = Directory.GetDirectories(this._SecondFolderPath);

                    foreach (string directory in directories)
                    {
                        Directory.Delete(directory);
                    }
                }

                //Copy first folder to the second
                if (Directory.Exists(this._FirstFolderPath))
                {
                    //Copy the files
                    string[] files = Directory.GetFiles(this._FirstFolderPath);

                    foreach (string s in files)
                    {
                        string fileName = Path.GetFileName(s);
                        string destFile = Path.Combine(this._SecondFolderPath, fileName);
                        File.Copy(s, destFile, true);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Impossible d'effectuer la synchronisation OneWay", ex);
            }
        }

        #endregion
    }
}
