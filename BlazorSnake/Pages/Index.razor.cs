using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorSnake.Pages
{
    public partial class Index
    {
        const int MatrixWidth=20;
        public int[,] matrix = new int[MatrixWidth, MatrixWidth];
        public int snakeLength = 3;
        public int directionRequest = 1;
        public int snakeDir=1;

        public List<coord> snake;
        public coord food;

        public bool live { get; set; } = false;
        int score;
        string key { get; set; }

        Boolean GameFinished;
        protected override void OnInitialized()
        {
            rand = new Random();

            StartGame();
            Paint();
        }


        async Task Run()
        {
            live=true;
            while (live)
            {
                await Task.Delay(100);

                var head = snake.FirstOrDefault();
                var target = directionRequest switch
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
                snakeDir=directionRequest;
                Paint();
                
            }

            
            score = snakeLength - 3;
            StateHasChanged();
        }

        Random rand;
        void CreateFood()
        {
            food = new coord();
            food.x = rand.Next(0, MatrixWidth);
            food.y = rand.Next(0, MatrixWidth);
            if (snake.Contains(food))
                CreateFood();

            score = snakeLength - 3;
            StateHasChanged();
        }

        void CheckWallOrBody(coord target)
        {
            live = !(target.x < 0 || target.x >= MatrixWidth || target.y < 0 || target.y >= MatrixWidth);
            if (snake.Contains(target))
                live = false;
            GameFinished=!live;
        }

        public void Paint()
        {
            try
            {
                Array.Clear(matrix, 0, MatrixWidth* MatrixWidth);

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
        ElementReference mydiv;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await js.InvokeVoidAsync("SetFocusToElement", mydiv);
            }
        }
        protected async Task KeyDown(KeyboardEventArgs e)
        {
            key=e.Key;
            if (!live && !GameFinished) { 
                StartGame();
                Run();
            }
            directionRequest = (e.Key, snakeDir) switch
            {
                ("w" or "ArrowUp",    1 or 3) => 2,
                ("s" or "ArrowDown",  1 or 3) => 4,
                ("a" or "ArrowLeft",  2 or 4) => 1,
                ("d" or "ArrowRight", 2 or 4) => 3,
                _ => directionRequest
            };
        }
        void StartGame()
        {
            food = new coord();
            directionRequest=1;
            snakeLength=3;
            snake = new List<coord>();
            snake.Add(new coord() { x = 5, y = 5 });
            snake.Add(new coord() { x = 6, y = 5 });
            snake.Add(new coord() { x = 7, y = 5 });
            CreateFood();
            GameFinished=false;
            
            
        }

        async Task Restart()
        {
            StartGame();
            Paint();
            await js.InvokeVoidAsync("SetFocusToElement", mydiv);
        }




        public class coord
        {
            public int x { get; set; }
            public int y { get; set; }

            public override bool Equals(object obj)
            {
                return ((coord)obj).x == x && ((coord)obj).y == y;
            }

            public override int GetHashCode()
            {
                return x*10+y;
            }
        }

    }
}