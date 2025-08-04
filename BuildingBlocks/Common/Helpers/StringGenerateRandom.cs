namespace BuildingBlocks.Common.Helpers
{
    public static class StringGenerateRandom
    {
        const string Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        private static readonly ThreadLocal<Random> Random = new(() => new Random());

        public static string Generate(int length = 18)
        {
            return new string(Enumerable.Repeat(Chars, length)
                .Select(s => s[Random.Value.Next(s.Length)]).ToArray());
        }
    }
}
