using System;
using System.Collections.Generic;

namespace GetCNPJ.Providers.CNPJA
{
    internal class CNPJAResponse
    {
        public DateTime? updated { get; set; }
        public string taxId { get; set; }
        public string alias { get; set; }
        public DateTime? founded { get; set; }
        public bool head { get; set; }
        public CompanyCNPJA company { get; set; }
        public DateTime? statusDate { get; set; }
        public StatusCNPJA status { get; set; }
        public AddressCNPJA address { get; set; }
        public ActivityCNPJA mainActivity { get; set; }
        public List<PhoneCNPJA> phones { get; set; }
        public List<EmailCNPJA> emails { get; set; }
        public List<ActivityCNPJA> sideActivities { get; set; }
    }

    internal class CompanyCNPJA
    {
        public List<MemberCNPJA> members { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public decimal? equity { get; set; }
        public NatureCNPJA nature { get; set; }
        public SizeCNPJA size { get; set; }
        public SimplesCNPJA simples { get; set; }
        public SimplesCNPJA simei { get; set; }
    }

    internal class MemberCNPJA
    {
        public DateTime? since { get; set; }
        public PersonCNPJA person { get; set; }
        public RoleCNPJA role { get; set; }
    }

    internal class PersonCNPJA
    {
        public string id { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string taxId { get; set; }
        public string age { get; set; }
    }

    internal class RoleCNPJA
    {
        public int id { get; set; }
        public string text { get; set; }
    }

    internal class NatureCNPJA
    {
        public int id { get; set; }
        public string text { get; set; }
    }

    internal class SizeCNPJA
    {
        public int id { get; set; }
        public string acronym { get; set; }
        public string text { get; set; }
    }

    internal class SimplesCNPJA
    {
        public bool optant { get; set; }
        public DateTime? since { get; set; }
    }

    internal class StatusCNPJA
    {
        public int id { get; set; }
        public string text { get; set; }
    }

    internal class AddressCNPJA
    {
        public int municipality { get; set; }
        public string street { get; set; }
        public string number { get; set; }
        public string district { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string details { get; set; }
        public string zip { get; set; }
        public CountryCNPJA country { get; set; }
    }

    internal class CountryCNPJA
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    internal class ActivityCNPJA
    {
        public int id { get; set; }
        public string text { get; set; }
    }

    internal class PhoneCNPJA
    {
        public string type { get; set; }
        public string area { get; set; }
        public string number { get; set; }
    }

    internal class EmailCNPJA
    {
        public string ownership { get; set; }
        public string address { get; set; }
        public string domain { get; set; }
    }
}
