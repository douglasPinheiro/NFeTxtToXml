﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace NFe.Components.GovDigital
{
    public class GovDigital : GovDigitalBase
    {
        #region Construtores
        public GovDigital(TipoAmbiente tpAmb, string pastaRetorno, X509Certificate certificate, int codMun, string proxyUser, string proxyPass, string proxyServer)
            : base(tpAmb, pastaRetorno, certificate, codMun, proxyUser, proxyPass, proxyServer)
        { }
        #endregion
    }
}
