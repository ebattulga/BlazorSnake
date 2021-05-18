using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorSnake.Pages
{
    public partial class Index
    {
        public int[,] matrix = new int[10, 10];
        public int snakeLength = 3;
        public int snakeDir = 1;
        public List<coord> snake;
        public coord food;
        public bool live { get; set; } = true;
        int score;
        string key { get; set; }
        protected override void OnInitialized()
        {
            //matrix[2, 3] = 1;
            //matrix[2, 4] = 1;
            //matrix[2, 5] = 1;
            rand = new Random();
            food = new coord();
            snake = new List<coord>();
            snake.Add(new coord() { x = 3, y = 5 });
            snake.Add(new coord() { x = 4, y = 5 });
            snake.Add(new coord() { x = 5, y = 5 });
            CreateFood();
            

        }

        protected override async Task OnInitializedAsync()
        {

            Paint();
            await Run();
        }


        async Task Run()
        {
            while (live)
            {
                await Task.Delay(500);

                var head = snake.FirstOrDefault();
                var target = snakeDir switch
                {
                    1 => new coord() { x = head.x - 1, y = head.y },
                    2 => new coord() { x = head.x, y = head.y - 1 },
                    3 => new coord() { x = head.x + 1, y = head.y },
                    4 => new coord() { x = head.x, y = head.y + 1 },
                    _ => null
                };
                CheckWallOrBody(target);
                Boolean eat = target.Equals(food);
                snake.Insert(0, target);
                if (!eat)
                    snake.RemoveAt(snakeLength);
                else
                {
                    CreateFood();
                    snakeLength++;
                    score = snakeLength - 3;
                    StateHasChanged();
                }
                Paint();
                
            }

            
            score = snakeLength - 3;
            StateHasChanged();
        }

        Random rand;
        void CreateFood()
        {
            food = new coord();
            food.x = rand.Next(0, 10);
            food.y = rand.Next(0, 10);
            if (snake.Contains(food))
                CreateFood();

            score = snakeLength - 3;
            StateHasChanged();
        }

        void CheckWallOrBody(coord target)
        {
            live = !(target.x < 0 || target.x >= 10 || target.y < 0 || target.y >= 10);
            if (snake.Contains(target))
                live = false;
            
        }

        public void Paint()
        {
            try
            {
                Array.Clear(matrix, 0, 100);

                foreach (var item in snake)
                {
                    matrix[item.x, item.y] = 1;
                }
                matrix[food.x, food.y] = 1;
            }
            catch (Exception)
            {
                live=false;
            }

            StateHasChanged();
        }

        protected void KeyDown(KeyboardEventArgs e)
        {
            snakeDir = (e.Key, snakeDir) switch
            {
                ("w" or "ArrowUp", 1 or 3) => 2,
                ("s" or "ArrowArrowDown", 1 or 3) => 4,
                ("a" or "ArrowLeft", 2 or 4) => 1,
                ("d" or "ArrowRight", 2 or 4) => 3,
                _ => snakeDir
            };

        }

        protected ElementReference myDiv;


        public class coord
        {
            public int x { get; set; }
            public int y { get; set; }

            public override bool Equals(object obj)
            {
                return ((coord)obj).x == x && ((coord)obj).y == y;
            }
        }

    }
}