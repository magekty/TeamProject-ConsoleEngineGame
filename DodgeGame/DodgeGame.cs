using ConsoleEngine;

// 피하기 게임 — 입문판. 규칙은 표준판과 같고, 시간을 '초' 대신 '프레임'으로 센다.
// 초당 10프레임이므로: 돌은 프레임마다 1칸(초당 10칸), 3프레임마다 생성(0.3초),
// 600프레임(60초)을 버티면 승리.
class DodgeGame : GameBase
{
    private const int SpawnEveryFrames = 3;   // 규칙 3: 0.3초마다 돌 하나
    private const int SpawnItemFrames = 50;   // 규칙 4: 5초마다 아이템 하나
    private const int GoalSeconds = 60;       // 최종 목표: 60초 버티기

    private readonly List<Rock> _rocks = new List<Rock>();
    private readonly List<Item> _items = new List<Item>();
    private int _playerX;
    private int _frame;                       // 지금까지 지난 프레임 수
    private GameState _state = GameState.Playing;

    private int _rockClearItemCount = 0;
    private int RockClearItemCount
    {  get { return _rockClearItemCount; }
       set { _rockClearItemCount = value > 3 ? 3 : value; } // 아이템 갯수제한 3개
    }


    public DodgeGame() : base(30, 20)         // 규칙 1: 30×20칸, 초당 10프레임
    {
        _playerX = Width / 2;                 // 맨 아래 줄 가운데
    }

    protected override void Input(ConsoleKey key)
    {
        if (key == ConsoleKey.Escape) Quit();            // 규칙 6
        if (_state != GameState.Playing)
        {
            if (key == ConsoleKey.R) Restart();
            return;
        }
        if (key == ConsoleKey.LeftArrow && _playerX > 0) _playerX--;
        if (key == ConsoleKey.RightArrow && _playerX < Width - 1)
            _playerX++;                                   // 규칙 2
        if (key == ConsoleKey.Spacebar&&_rockClearItemCount>=1)
        {
            RockClearItem();
            _rockClearItemCount--;
        }
    }

    private void Restart()
    {
        _rocks.Clear();
        _playerX = Width / 2;
        _frame = 0;
        _state = GameState.Playing;
        _rockClearItemCount = 0;
    }

    protected override void Update()
    {
        if (_state != GameState.Playing) return;
        _frame++;
        if (_frame >= GoalSeconds * FramesPerSecond)     // 규칙 5
        {
            _state = GameState.Clear;
            return;
        }

        // 돌과 아이템의 스폰 타이밍이 겹칠때 X좌표가 똑같아서 아이템을 먹고 게임오버하는 상황을 방지
        if (_frame % SpawnEveryFrames == 0&& _frame % SpawnItemFrames == 0) 
        {
            int rockX = Random.Shared.Next(Width);
            _rocks.Add(new Rock(rockX));
            int itemX = Random.Shared.Next(Width);
            while(rockX == itemX) //X좌표가 같으면 같지 않게 될때까지 반복
                itemX = Random.Shared.Next(Width);
            _items.Add(new Item(itemX));
        }
        else 
        {
            if (_frame % SpawnEveryFrames == 0)
                _rocks.Add(new Rock(Random.Shared.Next(Width)));
            if (_frame % SpawnItemFrames == 0)
                _items.Add(new Item(Random.Shared.Next(Width)));
        }

        
        for (int i = _rocks.Count - 1; i >= 0; i--)
        {
            Rock rock = _rocks[i];
            rock.Y++;                                     // 한 프레임에 한 칸
            if (rock.Y == Height - 1 && rock.X == _playerX)
                _state = GameState.GameOver;              // 규칙 4
            if (rock.Y >= Height) _rocks.RemoveAt(i);
        }
        for(int i= _items.Count - 1; i>=0; i--)
        {
            Item item = _items[i];
            item.Y++;
            if (item.Y == Height - 1 && item.X == _playerX)
            {
                RockClearItemCount++;             // 규칙 5
                _items.RemoveAt(i);
            }
            if (item.Y >= Height) _items.RemoveAt(i);
        }
    }

    protected override void Render()
    {
        foreach (Rock rock in _rocks)
            Draw(rock.X, rock.Y, '*');
        foreach (Item item in _items)
            Draw(item.X, item.Y, 'O');
        Draw(_playerX, Height - 1, '@');

        // 한글은 화면에서 2칸을 차지한다(이슈 #9). 그래서 글은 맨 나중에,
        // 그 줄의 오른쪽에 아무것도 그리지 않는 자리에만 쓴다.
        DrawText(0, 0, $"시간 {_frame / FramesPerSecond} / {GoalSeconds}초   남은 아이템:{_rockClearItemCount}");
        if (_state == GameState.GameOver)
            DrawText(5, 9, "게임 오버 - R: 다시");
        if (_state == GameState.Clear)
            DrawText(6, 9, "성공! - R: 다시");
    }


    // 아이템 효과:화면내에 모든 돌 제거,돌만 제거하고 아이템은 건드리지 않음
    private void RockClearItem()
    {
        _rocks.Clear();
    }

    
}
