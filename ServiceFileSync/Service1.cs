using ServiceFileSync.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ServiceFileSync
{
    public partial class Service1 : ServiceBase
    {
        #region Fields

        /// <summary>
        ///     Instance du service.
        /// </summary>
        private readonly MonService _Service;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initialise une nouvelle instance de la classe <see cref="Service1"/>.
        /// </summary>
        public Service1()
        {
            InitializeComponent();

            // TOCHANGE
            this._Service = new MonService(@"C:\TMP\INPUT", @"C:\TMP\OUTPUT");
            //Permet d'accepter la mise en pause et la reprise du service.
            this.CanPauseAndContinue = true;
        }

        #endregion

        #region Methods

        protected override void OnStart(string[] args) => this._Service.Start();

        protected override void OnStop() => this._Service.Stop();

        protected override void OnPause() => this._Service.Pause();

        protected override void OnContinue() => this._Service.Continue();

        #endregion
    }
}
