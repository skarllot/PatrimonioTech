using System.ComponentModel;

namespace PatrimonioTech.Domain.Ativos;

public enum B3TickerCode
{
    [Description("Direito de subscrição a uma ação ordinária")]
    DireitoSubcricaoAcaoOrdinaria = 1,

    [Description("Direito de subscrição a uma ação preferencial")]
    DireitoSubcricaoAcaoPreferencial = 2,

    [Description("Ação ordinária (ON)")] AcaoOrdinaria = 3,

    [Description("Ação preferencial (PN)")]
    AcaoPreferencial = 4,

    [Description("Ação preferencial classe A")]
    AcaoPreferencialClasseA = 5,

    [Description("Ação preferencial classe B")]
    AcaoPreferencialClasseB = 6,

    [Description("Ação preferencial classe C")]
    AcaoPreferencialClasseC = 7,

    [Description("Ação preferencial classe D")]
    AcaoPreferencialClasseD = 8,

    [Description("Recibo de subscrição sobre uma ação ordinária")]
    ReciboSubscricaoAcaoOrdinaria = 9,

    [Description("Recibo de subscrição sobre uma ação preferencial")]
    ReciboSubscricaoAcaoPreferencial = 10,

    [Description("Unit, ETF, Fundo Imobiliário ou BDR")]
    Fundo = 11,

    [Description("BDR Patrocinado Nível II")]
    BdrPatrocinadoN2 = 32,

    [Description("BDR Patrocinado Nível III")]
    BdrPatrocinadoN3 = 33,

    [Description("BDR Não Patrocinado")] BdrNaoPatrocinadoA = 34,
    [Description("BDR Não Patrocinado")] BdrNaoPatrocinadoB = 35,
    [Description("BDR de ETF")] BdrEtf = 39
}
