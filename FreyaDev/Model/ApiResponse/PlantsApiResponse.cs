namespace FreyaDev.Model
{
    internal class PlantsApiResponse : IApiResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }

        public List<Plant> Data { get; set; }
    }
}
