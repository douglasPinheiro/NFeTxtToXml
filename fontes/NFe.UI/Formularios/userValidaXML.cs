﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

using NFe.Settings;
using NFe.Components;
using NFe.Certificado;

namespace NFe.UI
{
    public partial class userValidaXML : UserControl1
    {
        private int Emp;
        private FileInfo oArqDestino = null;
        private WebBrowser wb = null;

        public userValidaXML()
        {
            InitializeComponent();
        }

        public override void UpdateControles()
        {
            base.UpdateControles();

            this.cbEmpresas.SelectedIndexChanged -= cbEmpresas_SelectedIndexChanged;
            this.cbEmpresas.DisplayMember = NFe.Components.NFeStrConstants.Nome;
            this.cbEmpresas.ValueMember = "Key";
            this.cbEmpresas.DataSource = Auxiliar.CarregaEmpresa(false);
            this.cbEmpresas.SelectedIndexChanged += cbEmpresas_SelectedIndexChanged;

            int posicao = uninfeDummy.xmlParams.ReadValue(this.GetType().Name, "last_empresa", 0);
            if (posicao >= (this.cbEmpresas.DataSource as System.Collections.ArrayList).Count)
                posicao = 0;

            this.cbEmpresas.SelectedIndex = posicao;
            cbEmpresas_SelectedIndexChanged(null, null);
            this.btn_Validar.Enabled = false;

            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
            t.Interval = 50;
            t.Tick += (sender, e) =>
            {
                ((System.Windows.Forms.Timer)sender).Stop();
                ((System.Windows.Forms.Timer)sender).Dispose();

                edtFilename.Focus();
            };
            t.Start();
        }

        private void edtFilename_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                MetroFramework.Controls.MetroTextBox control = (MetroFramework.Controls.MetroTextBox)sender;
                int x = control.ClientRectangle.Width - control.Icon.Size.Width;
                if (e.Location.X >= x)
                {
                    string path = uninfeDummy.xmlParams.ReadValue(this.GetType().Name, "path", "");

                    using (OpenFileDialog dlg = new OpenFileDialog())
                    {
                        dlg.RestoreDirectory = true;
                        dlg.Filter = "";

                        if (Empresas.Configuracoes[this.Emp].Servico == TipoAplicativo.Nfse)
                        {
                            dlg.Filter += "Arquivos de NFSe|*" + Propriedade.ExtEnvio.EnvLoteRps +
                                ";*" + Propriedade.ExtEnvio.PedCanNfse +
                                ";*" + Propriedade.ExtEnvio.PedLoteRps +
                                ";*" + Propriedade.ExtEnvio.PedSitLoteRps +
                                ";*" + Propriedade.ExtEnvio.PedSitNfse +
                                ";*" + Propriedade.ExtEnvio.PedSitNfseRps +
                                ";*" + Propriedade.ExtEnvio.PedInuNfse +
                                ";*" + Propriedade.ExtEnvio.PedNFSePNG + 
                                ";*" + Propriedade.ExtEnvio.PedNFSePDF; 
                        }
                        else
                        {
                            dlg.Filter = "Todos os arquivos|*" + Propriedade.ExtEnvio.Nfe +
                                        ";*" + Propriedade.ExtEnvio.Cte +
                                        ";*" + Propriedade.ExtEnvio.EnvCancelamento_XML +
                                        ";*" + Propriedade.ExtEnvio.EnvCCe_XML +
                                        ";*" + Propriedade.ExtEnvio.EnvManifestacao_XML +
                                        ";*" + Propriedade.ExtEnvio.EnvDFe_XML +
                                //";*" + Propriedade.ExtEnvio.ConsNFeDest_XML +
                                //";*" + Propriedade.ExtEnvio.EnvDownload_XML +
                                //";*" + Propriedade.ExtEnvio.EnvCancRegistroDeSaida_XML +
                                //";*" + Propriedade.ExtEnvio.EnvRegistroDeSaida_XML +
                                        ";*" + Propriedade.ExtEnvio.PedEve +
                                        ";*" + Propriedade.ExtEnvio.PedInu_XML +
                                        ";*" + Propriedade.ExtEnvio.PedSit_XML;
                            dlg.Filter += string.Format("|Arquivos da NFe/NFCe (*.*{0})|*{0}", Propriedade.ExtEnvio.Nfe);
                            dlg.Filter += string.Format("|Arquivos de CTe (*.*{0})|*{0}", Propriedade.ExtEnvio.Cte);
                            dlg.Filter += string.Format("|Arquivos de DFe (*.*{0})|*{0}", Propriedade.ExtEnvio.EnvDFe_XML);
                            dlg.Filter += string.Format("|Arquivos de MDFe (*.*{0})|*{0}", Propriedade.ExtEnvio.MDFe);
                            dlg.Filter += string.Format("|Arquivos de eventos (*.*{0},*.*{1},*.*{2},*.*{3},*.*{4})|*{0};*{1};*{2};*{3};*{4}",
                                Propriedade.ExtEnvio.EnvCCe_XML,
                                Propriedade.ExtEnvio.PedEve,
                                Propriedade.ExtEnvio.EnvCancelamento_XML,
                                Propriedade.ExtEnvio.EnvDownload_XML,
                                Propriedade.ExtEnvio.EnvManifestacao_XML);
                        }
                        
                        if (!string.IsNullOrEmpty(path))
                            dlg.InitialDirectory = path;

                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            if (!string.IsNullOrEmpty(dlg.FileName))
                            {
                                uninfeDummy.xmlParams.WriteValue(this.GetType().Name, "path", Path.GetDirectoryName(dlg.FileName));
                                uninfeDummy.xmlParams.Save();

                                this.edtFilename.Text = dlg.FileName;

                                btn_Validar_Click(null, null);
                            }
                        }
                    }
                }
            }
        }

        private void cbEmpresas_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Emp = -1;
            try
            {
                if (this.cbEmpresas.SelectedValue != null)
                {
                    var list = (this.cbEmpresas.DataSource as System.Collections.ArrayList)[this.cbEmpresas.SelectedIndex] as NFe.Components.ComboElem;
                    this.Emp = Empresas.FindConfEmpresaIndex(list.Valor, NFe.Components.EnumHelper.StringToEnum<TipoAplicativo>(list.Servico));

                    uninfeDummy.xmlParams.WriteValue(this.GetType().Name, "last_empresa", this.cbEmpresas.SelectedIndex);
                    uninfeDummy.xmlParams.Save();
                }
            }
            catch (Exception ex)
            {
                MetroFramework.MetroMessageBox.Show(uninfeDummy.mainForm, ex.Message, "");
            }
            finally
            {
                this.edtFilename.Enabled =
                    this.btn_Validar.Enabled = this.Emp >= 0;
            }
        }

        private void textBox_arqxml_TextChanged(object sender, EventArgs e)
        {
            LimparEPosicionarTC();
            this.btn_Validar.Enabled = !string.IsNullOrEmpty(this.edtFilename.Text);
        }

        private void LimparEPosicionarTC()
        {
            this.metroTabControl.SelectedIndex = 0;
            this.textBox_resultado.Clear();
            this.edtTipoarquivo.Clear();
        }

        private void btn_Validar_Click(object sender, EventArgs e)
        {
            LimparEPosicionarTC();

            try
            {
                if (this.edtFilename.Text == "" || !File.Exists(this.edtFilename.Text))
                {
                    this.textBox_resultado.Text = "Arquivo não encontrado.";
                    return;
                }

                //Copiar o arquivo XML para temporários para assinar e depois vou validar o que está nos temporários
                FileInfo oArquivo = new FileInfo(this.edtFilename.Text);
                string cArquivo = System.IO.Path.GetTempPath() + oArquivo.Name;

                oArqDestino = new FileInfo(cArquivo);

                oArquivo.CopyTo(cArquivo, true);

                //Remover atributo de somente leitura que pode gerar problemas no acesso do arquivo
                NFe.Service.TFunctions.RemoveSomenteLeitura(cArquivo);

                NFe.Service.TaskValidar val = new Service.TaskValidar();
                val.NomeArquivoXML = oArqDestino.FullName;
                val.Execute();

                //Detectar o tipo do arquivo
                NFe.Validate.ValidarXML validarXML = new NFe.Validate.ValidarXML(cArquivo, Empresas.Configuracoes[Emp].UnidadeFederativaCodigo);

                this.textBox_resultado.Text = validarXML.TipoArqXml.cRetornoTipoArq;

                if (validarXML.TipoArqXml.nRetornoTipoArq >= 1 && validarXML.TipoArqXml.nRetornoTipoArq <= SchemaXML.MaxID)
                {
                    this.edtTipoarquivo.Text = validarXML.TipoArqXml.cRetornoTipoArq;

                    //Assinar o arquivo XML copiado para a pasta TEMP
                    bool lValidar = false;
                    AssinaturaDigital oAD = new AssinaturaDigital();
                    try
                    {
                        validarXML.EncryptAssinatura(cArquivo); //danasa: 12/2013
                        oAD.Assinar(cArquivo, Emp, Empresas.Configuracoes[Emp].UnidadeFederativaCodigo);
                        lValidar = true;
                    }
                    catch (Exception ex)
                    {
                        lValidar = false;
                        this.textBox_resultado.Text = "Ocorreu um erro ao tentar assinar o XML: \r\n\r\n" +
                            validarXML.TipoArqXml.cRetornoTipoArq + "\r\n" + ex.Message;
                    }

                    if (lValidar == true)
                    {
                        //Validar o arquivo
                        if (validarXML.TipoArqXml.nRetornoTipoArq >= 1 && validarXML.TipoArqXml.nRetornoTipoArq <= SchemaXML.MaxID)
                        {
                            ///danasa: 12/2013
                            validarXML.ValidarArqXML(cArquivo);
                            if (string.IsNullOrEmpty(validarXML.TipoArqXml.cArquivoSchema))
                            {
                                this.textBox_resultado.Text = "XML não possui schema de validação, sendo assim não é possível validar XML";
                            }
                            else if (validarXML.Retorno == 0)
                            {
                                this.textBox_resultado.Text = "Arquivo validado com sucesso!";
                            }
                            else
                            {
                                this.textBox_resultado.Text = "XML INCONSISTENTE!\r\n\r\n" + validarXML.RetornoString;
                            }
                        }
                        else
                        {
                            //this.textBox_tipoarq.Text = validarXML.TipoArqXml.cRetornoTipoArq;
                            this.textBox_resultado.Text = "XML INCONSISTENTE!\r\n\r\n" + validarXML.TipoArqXml.cRetornoTipoArq;
                        }
                    }
                }

                try
                {
                    if (wb == null)
                    {
                        wb = new WebBrowser();
                        wb.Parent = this.metroTabPage2;
                        wb.Dock = DockStyle.Fill;
                        wb.DocumentCompleted += webBrowser1_DocumentCompleted;
                    }
                    wb.Navigate(cArquivo);
                }
                catch { 
                    webBrowser1_DocumentCompleted(null,null); 
                }
            }
            catch (Exception ex)
            {
                this.textBox_resultado.Text = ex.Message + "\r\n" + ex.StackTrace;
            }
        }

        void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (oArqDestino != null)
                if (oArqDestino.Exists)
                    oArqDestino.Delete();

            oArqDestino = null;
        }
    }
}
