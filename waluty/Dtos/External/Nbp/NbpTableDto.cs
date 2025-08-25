namespace waluty.Dtos.External.Nbp
{
    public class NbpTableDto
    {
        public string No { get; set; } = "";
        public DateTime EffectiveDate { get; set; }
        public List<NbpRateDto> Rates { get; set; } = new();
    }
}
