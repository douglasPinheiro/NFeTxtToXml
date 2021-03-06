﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NFe.Components;
using NFe.Settings;

namespace NFe.UI
{
    public partial class userSobre : UserControl1
    {
        public userSobre()
        {
            InitializeComponent();
        }

        public override void UpdateControles()
        {
            base.UpdateControles();

            switch (NFe.Components.Propriedade.TipoAplicativo)
            {
                //case NFe.Components.TipoAplicativo.Nfe:
                default:
                    break;

                case NFe.Components.TipoAplicativo.Nfse:
                    this.pictureBox2.Image = NFe.UI.Properties.Resources.uninfse128;
                    break;
            }
            this.textBox_versao.Text = Propriedade.Versao;
            lblDescricaoAplicacao.Text = Propriedade.DescricaoAplicacao;
            lblNomeAplicacao.Text = Propriedade.NomeAplicacao;
            this.labelTitle.Text = "Sobre o " + Propriedade.NomeAplicacao;

            //Atualizar o texto da licença de uso
            this.textBox_licenca.Text = "GNU General Public License\r\n\r\n";
            this.textBox_licenca.Text += Propriedade.NomeAplicacao + " - " + Propriedade.DescricaoAplicacao + "\r\n";
            this.textBox_licenca.Text += string.Format("Copyright (C) 2008-{0} {1}", DateTime.Today.Year, ConfiguracaoApp.NomeEmpresa) + "\r\n\r\n";
            this.textBox_licenca.Text += "Este programa é software livre; você pode redistribuí-lo e/ou modificá-lo sob os termos da Licença Pública Geral GNU, conforme publicada pela Free Software Foundation; tanto a versão 2 da Licença como (a seu critério) qualquer versão mais nova.\r\n\r\n";
            this.textBox_licenca.Text += "Este programa é distribuído na expectativa de ser útil, mas SEM QUALQUER GARANTIA; sem mesmo a garantia implícita de COMERCIALIZAÇÃO ou de ADEQUAÇÃO A QUALQUER PROPÓSITO EM PARTICULAR. Consulte a Licença Pública Geral GNU para obter mais detalhes.\r\n\r\n";
            this.textBox_licenca.Text += "Você deve ter recebido uma cópia da Licença Pública Geral GNU junto com este programa; se não, escreva para a Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA     02111-1307, USA ou consulte a licença oficial em http://www.gnu.org/licenses/.";

            textBox_DataUltimaModificacao.Text = System.IO.File.GetLastWriteTime(Propriedade.NomeAplicacao + ".exe").ToString("dd/MM/yyyy - HH:mm:ss");

            lblEmpresa.Text = ConfiguracaoApp.NomeEmpresa;
            linkLabelSite.Visible = !string.IsNullOrEmpty(ConfiguracaoApp.Site);
            linkLabelSiteProduto.Visible = !string.IsNullOrEmpty(ConfiguracaoApp.SiteProduto);
            linkLabelEmail.Visible = !string.IsNullOrEmpty(ConfiguracaoApp.Email);

            linkLabelSite.Text = ConfiguracaoApp.Site;
            linkLabelSiteProduto.Text = ConfiguracaoApp.SiteProduto;
            linkLabelEmail.Text = ConfiguracaoApp.Email;

            string elapsedDays = ConfiguracaoApp.ExecutionTime.Elapsed.Days + " dias ininterruptos.";

            if (ConfiguracaoApp.ExecutionTime.Elapsed.Days < 1)
                elapsedDays = ConfiguracaoApp.ExecutionTime.Elapsed.Hours + " horas ininterruptas.";

            if (ConfiguracaoApp.ExecutionTime.Elapsed.Hours < 1)
                elapsedDays = "A menos de uma hora.";

            txtElapsedDays.Text = elapsedDays;
        }

        private void linkLabelSite_Click(object sender, EventArgs e)
        {
            try
            {
                string url = (sender as MetroFramework.Controls.MetroLink).Text;
                if (url.Contains("@"))
                    url = "mailto:" + url;
                else
                    url = "http://" + url;
                System.Diagnostics.Process.Start(url);
            }
            catch (Exception ex)
            {
                MetroFramework.MetroMessageBox.Show(uninfeDummy.mainForm, ex.Message, "");
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            try
            {
                NFe.Components.Functions.ExibeDocumentacao();
            }
            catch (Exception ex)
            {
                MetroFramework.MetroMessageBox.Show(uninfeDummy.mainForm, ex.Message, "");
            }
        }

        private void userSobre_Load(object sender, EventArgs e)
        {
        }
    }
}
