using System.ComponentModel;

namespace Application.Features.Onboarding.Domain
{
    public enum TypeCorporateSizing
    {

        [Description("Pequeno porte ou ME e EPP")]
        SmallBusiness,

        [Description("Empresas de Médio Porte - até 20 milhões")]
        MediumBusiness,

        [Description("Grande Empresa")]
        LargeBusiness,

    }
}