using System;

namespace IndependentWork1
{
    public class Recipe
    {
        private string _title;
        private int _cookingTimeMinutes;
        private int _servings;

        public string Title
        {
            get => _title;
            set => _title = string.IsNullOrWhiteSpace(value) ? "Без назви" : value;
        }
        public int Servings => _servings;

        public Recipe(string title, int cookingTimeMinutes, int servings)
        {
            Title = title;
            _cookingTimeMinutes = cookingTimeMinutes > 0 ? cookingTimeMinutes : 10;
            _servings = servings > 0 ? servings : 1;
        }

        public double GetTimePerServing()
        {
            return (double)_cookingTimeMinutes / _servings;
        }

        public void PrintRecipeInfo()
        {
            Console.WriteLine($"Рецепт: \"{Title}\" | Час приготування: {_cookingTimeMinutes} хв. | Порцій: {Servings}");
        }
    }

    public class Playlist
    {
        private string _name;
        private int _totalDurationSeconds;
        private int _trackCount;

        public string Name
        {
            get => _name;
            set => _name = string.IsNullOrWhiteSpace(value) ? "Мій плейліст" : value;
        }

        public int TrackCount => _trackCount; // Read-only властивість

        public Playlist(string name, int totalDurationSeconds, int trackCount)
        {
            Name = name;
            _totalDurationSeconds = totalDurationSeconds > 0 ? totalDurationSeconds : 0;
            _trackCount = trackCount >= 0 ? trackCount : 0;
        }

        public string GetFormattedDuration()
        {
            int minutes = _totalDurationSeconds / 60;
            int seconds = _totalDurationSeconds % 60;
            return $"{minutes} хв {seconds} сек";
        }

        public bool IsEmpty()
        {
            return _trackCount == 0;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("= ДЕМОНСТРАЦІЯ РОБОТИ КЛАСІВ \n");

            Console.WriteLine("- 1. Клас Recipe (Рецепт) ");
            Recipe borsch = new Recipe("Український Борщ", 90, 6);
            borsch.PrintRecipeInfo();
            Console.WriteLine($"Час приготування 1 порції: {borsch.GetTimePerServing():F1} хв.\n");

            Console.WriteLine("- 2. Клас Playlist (Плейліст) ");
            Playlist myPlaylist = new Playlist("Улюблені треки", 1485, 8);
            
            Console.WriteLine($"Назва плейліста: {myPlaylist.Name}");
            Console.WriteLine($"Кількість треків: {myPlaylist.TrackCount}");
            Console.WriteLine($"Загальна тривалість: {myPlaylist.GetFormattedDuration()}");
            Console.WriteLine($"Чи порожній плейліст: {(myPlaylist.IsEmpty() ? "Так" : "Ні")}");

            Console.WriteLine("\n= Виконання програми завершено успішно ");
        }
    }
}