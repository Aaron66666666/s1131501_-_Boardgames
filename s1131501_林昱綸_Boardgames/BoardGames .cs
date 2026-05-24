// ╔══════════════════════════════════════════════════════════╗
// ║  棋牌遊戲集  BoardGames.cs                              ║
// ║  包含：主選單 / 黑白棋 / 井字棋 / 五子棋               ║
// ║  相容 C# 7.3 / .NET Framework 4.7.2                    ║
// ╚══════════════════════════════════════════════════════════╝

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace s1131501_林昱綸_Boardgames
{
    // ════════════════════════════════════════════════════════
    //  音效（Windows 內建 Beep，不需安裝任何套件）
    // ════════════════════════════════════════════════════════
    static class SE
    {
        [DllImport("kernel32.dll")]
        static extern bool Beep(int f, int ms);

        static void P(Action a) => Task.Run(a);

        public static void Place()   => P(() => { try { Beep(880, 55); } catch { } });
        public static void Flip()    => P(() => { try { Beep(660, 35); Beep(990, 40); } catch { } });
        public static void Win()     => P(() => { try { foreach (int x in new[] { 523, 587, 659, 784, 1047 }) Beep(x, 90); } catch { } });
        public static void Draw()    => P(() => { try { Beep(440, 100); Beep(330, 140); } catch { } });
        public static void Invalid() => P(() => { try { Beep(220, 80); } catch { } });
    }

    // ════════════════════════════════════════════════════════
    //  主選單
    // ════════════════════════════════════════════════════════
    public class MainMenuForm : Form
    {
        public MainMenuForm()
        {
            Text            = "棋牌遊戲集";
            Size            = new Size(420, 500);
            StartPosition   = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox     = false;
            BackColor       = Color.FromArgb(245, 244, 238);
            Font            = new Font("Microsoft JhengHei UI", 10f);

            var lblTitle = new Label
            {
                Text      = "棋牌遊戲集",
                Font      = new Font("Microsoft JhengHei UI", 26f, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 38),
                AutoSize  = false,
                Bounds    = new Rectangle(0, 44, 420, 56),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lblSub = new Label
            {
                Text      = "Windows Programming (II)  ·  作業二",
                Font      = new Font("Microsoft JhengHei UI", 9f),
                ForeColor = Color.FromArgb(150, 148, 140),
                AutoSize  = false,
                Bounds    = new Rectangle(0, 98, 420, 24),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var sep = new Panel
            {
                BackColor = Color.FromArgb(210, 208, 198),
                Bounds    = new Rectangle(60, 130, 300, 1)
            };

            // 三個遊戲按鈕
            string[] labels = { "⚫  黑白棋    Reversi", "✕   井字棋    Tic-Tac-Toe", "●  五子棋    Gomoku" };
            Color[]  colors = { Color.FromArgb(15, 110, 86), Color.FromArgb(60, 52, 137), Color.FromArgb(153, 60, 29) };

            for (int i = 0; i < 3; i++)
            {
                var btn = new Button
                {
                    Text      = labels[i],
                    Font      = new Font("Microsoft JhengHei UI", 13f, FontStyle.Bold),
                    ForeColor = Color.White,
                    BackColor = colors[i],
                    FlatStyle = FlatStyle.Flat,
                    Bounds    = new Rectangle(60, 152 + i * 72, 300, 54),
                    Cursor    = Cursors.Hand,
                    Tag       = i
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.Click += GameBtn_Click;
                Controls.Add(btn);
            }

            var lblFooter = new Label
            {
                Text      = "視窗程式設計 (II)  ·  2025",
                Font      = new Font("Microsoft JhengHei UI", 9f),
                ForeColor = Color.FromArgb(190, 188, 180),
                AutoSize  = false,
                Bounds    = new Rectangle(0, 436, 420, 24),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Controls.AddRange(new Control[] { lblTitle, lblSub, sep, lblFooter });
        }

        void GameBtn_Click(object sender, EventArgs e)
        {
            int idx = (int)((Button)sender).Tag;
            Form f;
            if      (idx == 0) f = new ReversiForm();
            else if (idx == 1) f = new TicTacToeForm();
            else               f = new GomokuForm();
            f.Show();
        }
    }

    // ════════════════════════════════════════════════════════
    //  黑白棋  Reversi  (8×8)
    // ════════════════════════════════════════════════════════
    public class ReversiForm : Form
    {
        const int N    = 8;
        const int CELL = 58;
        const int MAR  = 14;

        int[] board = new int[N * N];
        int   turn  = 1;
        bool  over  = false;
        bool  vsAI  = false;
        int[] wins  = new int[3];

        static readonly int[] DR = { -1, -1, -1, 0, 0, 1, 1, 1 };
        static readonly int[] DC = { -1,  0,  1,-1, 1,-1, 0, 1 };

        Panel  pnlBoard;
        Label  lblStatus, lblBlackScore, lblWhiteScore;
        Button btnNew, btnMode;

        public ReversiForm()
        {
            Text            = "黑白棋 Reversi";
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox     = false;
            StartPosition   = FormStartPosition.CenterScreen;
            BackColor       = Color.FromArgb(245, 244, 238);
            Font            = new Font("Microsoft JhengHei UI", 10f);

            int boardPx = N * CELL + MAR * 2;
            ClientSize = new Size(boardPx + 180, boardPx + 28);

            pnlBoard = new Panel
            {
                Location  = new Point(MAR, MAR),
                Size      = new Size(N * CELL, N * CELL),
                BackColor = Color.FromArgb(34, 120, 78),
                Cursor    = Cursors.Hand
            };
            pnlBoard.Paint      += PnlBoard_Paint;
            pnlBoard.MouseClick += PnlBoard_Click;
            Controls.Add(pnlBoard);

            int px = MAR + N * CELL + 16;
            int py = MAR;

            lblStatus = NewLabel("", 11, FontStyle.Bold);
            lblStatus.SetBounds(px, py, 160, 30);

            lblBlackScore = NewLabel("", 10, FontStyle.Regular);
            lblBlackScore.SetBounds(px, py + 36, 160, 24);

            lblWhiteScore = NewLabel("", 10, FontStyle.Regular);
            lblWhiteScore.SetBounds(px, py + 60, 160, 24);

            btnMode = NewBtn("對戰：雙人", Color.FromArgb(15, 110, 86));
            btnMode.SetBounds(px, py + 100, 148, 36);
            btnMode.Click += (s, e) => { vsAI = !vsAI; NewGame(); };

            btnNew = NewBtn("新局", Color.FromArgb(90, 88, 80));
            btnNew.SetBounds(px, py + 146, 148, 36);
            btnNew.Click += (s, e) => NewGame();

            Controls.AddRange(new Control[] { lblStatus, lblBlackScore, lblWhiteScore, btnMode, btnNew });

            NewGame();
        }

        void NewGame()
        {
            board = new int[N * N];
            int m = N / 2;
            board[(m - 1) * N + (m - 1)] = 2;
            board[(m - 1) * N + m]       = 1;
            board[m * N + (m - 1)]       = 1;
            board[m * N + m]             = 2;
            turn = 1;
            over = false;
            btnMode.Text = vsAI ? "對戰：AI" : "對戰：雙人";
            RefreshUI();
        }

        void RefreshUI()
        {
            int bc = Count(1), wc = Count(2);
            lblBlackScore.Text = "⚫ 黑棋：" + bc + "  (勝 " + wins[1] + ")";
            lblWhiteScore.Text = "⚪ 白棋：" + wc + "  (勝 " + wins[2] + ")";

            if (over)
                lblStatus.Text = bc > wc ? "⚫ 黑棋獲勝！" : bc < wc ? "⚪ 白棋獲勝！" : "平局！";
            else
                lblStatus.Text = turn == 1 ? "輪到 ⚫ 黑棋" : "輪到 ⚪ 白棋";

            pnlBoard.Invalidate();
        }

        void PnlBoard_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Pen pen = new Pen(Color.FromArgb(20, 80, 50), 1f);
            for (int i = 0; i <= N; i++)
            {
                g.DrawLine(pen, i * CELL, 0, i * CELL, N * CELL);
                g.DrawLine(pen, 0, i * CELL, N * CELL, i * CELL);
            }
            pen.Dispose();

            // 星位
            int[] starPos = { N / 4, N * 3 / 4 };
            SolidBrush starBrush = new SolidBrush(Color.FromArgb(20, 80, 50));
            foreach (int r in starPos)
                foreach (int c in starPos)
                    g.FillEllipse(starBrush, c * CELL - 4, r * CELL - 4, 8, 8);
            starBrush.Dispose();

            List<int> valid = over ? new List<int>() : ValidMoves(turn);

            for (int r = 0; r < N; r++)
            for (int c = 0; c < N; c++)
            {
                int idx = r * N + c;
                int cx  = c * CELL + CELL / 2;
                int cy  = r * CELL + CELL / 2;
                int rad = CELL / 2 - 4;

                if (board[idx] == 1)
                    DrawPiece(g, cx, cy, rad, true);
                else if (board[idx] == 2)
                    DrawPiece(g, cx, cy, rad, false);
                else if (valid.Contains(idx))
                {
                    SolidBrush hb = new SolidBrush(Color.FromArgb(110, 220, 255, 180));
                    g.FillEllipse(hb, cx - 7, cy - 7, 14, 14);
                    hb.Dispose();
                }
            }
        }

        void DrawPiece(Graphics g, int cx, int cy, int r, bool black)
        {
            Color fill   = black ? Color.FromArgb(28, 28, 26)  : Color.FromArgb(248, 246, 238);
            Color border = black ? Color.FromArgb(60, 58, 52)  : Color.FromArgb(175, 173, 163);
            SolidBrush br = new SolidBrush(fill);
            g.FillEllipse(br, cx - r, cy - r, r * 2, r * 2);
            br.Dispose();
            Pen bp = new Pen(border, 1.5f);
            g.DrawEllipse(bp, cx - r, cy - r, r * 2, r * 2);
            bp.Dispose();
            int hr = r / 3;
            SolidBrush hb = new SolidBrush(Color.FromArgb(black ? 55 : 80, 255, 255, 255));
            g.FillEllipse(hb, cx - hr - 2, cy - hr - 2, hr * 2, hr * 2);
            hb.Dispose();
        }

        void PnlBoard_Click(object sender, MouseEventArgs e)
        {
            if (over) return;
            int c = e.X / CELL, r = e.Y / CELL;
            if (c < 0 || c >= N || r < 0 || r >= N) return;
            DoPlace(r * N + c);
        }

        void DoPlace(int idx)
        {
            if (over || board[idx] != 0) return;
            List<int> flips = Flips(idx, turn);
            if (flips.Count == 0) { SE.Invalid(); return; }

            board[idx] = turn;
            foreach (int f in flips) board[f] = turn;
            SE.Place();
            if (flips.Count > 2) SE.Flip();

            int next = turn == 1 ? 2 : 1;
            if      (ValidMoves(next).Count > 0) turn = next;
            else if (ValidMoves(turn).Count  > 0) { }
            else
            {
                over = true;
                int bc = Count(1), wc = Count(2);
                if (bc > wc) wins[1]++; else if (wc > bc) wins[2]++;
                SE.Win();
            }

            RefreshUI();

            if (!over && vsAI && turn == 2)
                Delay(420, AIMove);
        }

        void AIMove()
        {
            List<int> moves = ValidMoves(2);
            if (moves.Count == 0) return;

            int[] corners = { 0, N - 1, (N - 1) * N, N * N - 1 };
            foreach (int c in corners)
                if (moves.Contains(c)) { DoPlace(c); return; }

            int best = -1, bestFlips = -1;
            foreach (int m in moves)
            {
                int f = Flips(m, 2).Count;
                if (f > bestFlips) { bestFlips = f; best = m; }
            }
            DoPlace(best);
        }

        List<int> ValidMoves(int p)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < N * N; i++)
                if (board[i] == 0 && Flips(i, p).Count > 0) list.Add(i);
            return list;
        }

        List<int> Flips(int idx, int p)
        {
            int row = idx / N, col = idx % N, opp = p == 1 ? 2 : 1;
            List<int> all = new List<int>();
            for (int d = 0; d < 8; d++)
            {
                int r = row + DR[d], c = col + DC[d];
                List<int> line = new List<int>();
                while (r >= 0 && r < N && c >= 0 && c < N && board[r * N + c] == opp)
                { line.Add(r * N + c); r += DR[d]; c += DC[d]; }
                if (line.Count > 0 && r >= 0 && r < N && c >= 0 && c < N && board[r * N + c] == p)
                    all.AddRange(line);
            }
            return all;
        }

        int Count(int p) { int c = 0; foreach (int x in board) if (x == p) c++; return c; }

        static void Delay(int ms, Action cb)
        {
            Timer t = new Timer { Interval = ms };
            t.Tick += (s, e) => { t.Stop(); t.Dispose(); cb(); };
            t.Start();
        }

        static Label NewLabel(string text, float size, FontStyle fs) => new Label
        {
            Text      = text,
            Font      = new Font("Microsoft JhengHei UI", size, fs),
            AutoSize  = false,
            ForeColor = Color.FromArgb(44, 44, 42)
        };

        static Button NewBtn(string text, Color bg)
        {
            Button b = new Button
            {
                Text      = text,
                Font      = new Font("Microsoft JhengHei UI", 10f),
                FlatStyle = FlatStyle.Flat,
                BackColor = bg,
                ForeColor = Color.White,
                Cursor    = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }
    }

    // ════════════════════════════════════════════════════════
    //  井字棋  Tic-Tac-Toe  (3×3)
    // ════════════════════════════════════════════════════════
    public class TicTacToeForm : Form
    {
        const int CELL = 120;

        int[]  board   = new int[9];
        int    turn    = 1;
        bool   over    = false;
        bool   vsAI    = false;
        int[]  wins    = new int[3];
        int[]  winLine = null;

        static readonly int[][] LINES =
        {
            new[] {0,1,2}, new[] {3,4,5}, new[] {6,7,8},
            new[] {0,3,6}, new[] {1,4,7}, new[] {2,5,8},
            new[] {0,4,8}, new[] {2,4,6}
        };

        Panel  pnlBoard;
        Label  lblStatus, lblX, lblO;
        Button btnNew, btnMode;

        public TicTacToeForm()
        {
            Text            = "井字棋 Tic-Tac-Toe";
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox     = false;
            StartPosition   = FormStartPosition.CenterScreen;
            BackColor       = Color.FromArgb(245, 244, 238);
            Font            = new Font("Microsoft JhengHei UI", 10f);
            ClientSize      = new Size(3 * CELL + 180, 3 * CELL + 28);

            pnlBoard = new Panel
            {
                Location  = new Point(14, 14),
                Size      = new Size(3 * CELL, 3 * CELL),
                BackColor = Color.White,
                Cursor    = Cursors.Hand
            };
            pnlBoard.Paint      += PnlBoard_Paint;
            pnlBoard.MouseClick += PnlBoard_Click;
            Controls.Add(pnlBoard);

            int px = 14 + 3 * CELL + 18;
            int py = 14;

            lblStatus = NewLabel("", 12, FontStyle.Bold);
            lblStatus.SetBounds(px, py, 158, 30);

            lblX = NewLabel("", 10, FontStyle.Regular);
            lblX.SetBounds(px, py + 36, 158, 24);

            lblO = NewLabel("", 10, FontStyle.Regular);
            lblO.SetBounds(px, py + 60, 158, 24);

            btnMode = NewBtn("對戰：雙人", Color.FromArgb(60, 52, 137));
            btnMode.SetBounds(px, py + 100, 148, 36);
            btnMode.Click += (s, e) => { vsAI = !vsAI; NewGame(); };

            btnNew = NewBtn("新局", Color.FromArgb(90, 88, 80));
            btnNew.SetBounds(px, py + 146, 148, 36);
            btnNew.Click += (s, e) => NewGame();

            Controls.AddRange(new Control[] { lblStatus, lblX, lblO, btnMode, btnNew });

            NewGame();
        }

        void NewGame()
        {
            board = new int[9]; turn = 1; over = false; winLine = null;
            btnMode.Text = vsAI ? "對戰：AI" : "對戰：雙人";
            RefreshUI();
        }

        void RefreshUI()
        {
            lblX.Text = "❌ X  (勝 " + wins[1] + ")";
            lblO.Text = "⭕ O  (勝 " + wins[2] + ")";

            if (over)
                lblStatus.Text = winLine != null
                    ? (turn == 1 ? "❌ X 獲勝！" : "⭕ O 獲勝！")
                    : "平局！";
            else
                lblStatus.Text = turn == 1 ? "輪到 ❌ X" : "輪到 ⭕ O";

            pnlBoard.Invalidate();
        }

        void PnlBoard_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 格線
            Pen gridPen = new Pen(Color.FromArgb(185, 183, 173), 5f);
            gridPen.StartCap = LineCap.Round;
            gridPen.EndCap   = LineCap.Round;
            g.DrawLine(gridPen,  10,      CELL,     3 * CELL - 10, CELL);
            g.DrawLine(gridPen,  10,      2 * CELL, 3 * CELL - 10, 2 * CELL);
            g.DrawLine(gridPen,  CELL,    10,       CELL,          3 * CELL - 10);
            g.DrawLine(gridPen,  2*CELL,  10,       2 * CELL,      3 * CELL - 10);
            gridPen.Dispose();

            int pad = 26;
            for (int r = 0; r < 3; r++)
            for (int c = 0; c < 3; c++)
            {
                int idx = r * 3 + c;
                int cx  = c * CELL + CELL / 2;
                int cy  = r * CELL + CELL / 2;

                bool isWinCell = winLine != null && Array.IndexOf(winLine, idx) >= 0;
                if (isWinCell)
                {
                    SolidBrush hb = new SolidBrush(Color.FromArgb(28, 29, 158, 117));
                    g.FillRectangle(hb, c * CELL + 3, r * CELL + 3, CELL - 6, CELL - 6);
                    hb.Dispose();
                }

                if (board[idx] == 1)
                {
                    Pen xp = new Pen(Color.FromArgb(60, 52, 137), 10f);
                    xp.StartCap = LineCap.Round;
                    xp.EndCap   = LineCap.Round;
                    g.DrawLine(xp, cx - pad, cy - pad, cx + pad, cy + pad);
                    g.DrawLine(xp, cx + pad, cy - pad, cx - pad, cy + pad);
                    xp.Dispose();
                }
                else if (board[idx] == 2)
                {
                    Pen op = new Pen(Color.FromArgb(153, 60, 29), 10f);
                    g.DrawEllipse(op, cx - pad, cy - pad, pad * 2, pad * 2);
                    op.Dispose();
                }
            }

            // 獲勝連線
            if (winLine != null)
            {
                int a  = winLine[0], b = winLine[2];
                int ax = a % 3 * CELL + CELL / 2, ay = a / 3 * CELL + CELL / 2;
                int bx = b % 3 * CELL + CELL / 2, by = b / 3 * CELL + CELL / 2;
                Pen lp = new Pen(Color.FromArgb(190, 29, 158, 117), 6f);
                lp.StartCap = LineCap.Round;
                lp.EndCap   = LineCap.Round;
                g.DrawLine(lp, ax, ay, bx, by);
                lp.Dispose();
            }
        }

        void PnlBoard_Click(object sender, MouseEventArgs e)
        {
            if (over) return;
            int c = e.X / CELL, r = e.Y / CELL;
            if (c >= 3 || r >= 3) return;
            DoPlace(r * 3 + c);
        }

        void DoPlace(int idx)
        {
            if (over || board[idx] != 0) return;
            board[idx] = turn;
            SE.Place();

            winLine = FindWin(board);
            if (winLine != null)
            {
                over = true; wins[turn]++;
                SE.Win();
            }
            else if (IsFull())
            {
                over = true;
                SE.Draw();
            }
            else turn = turn == 1 ? 2 : 1;

            RefreshUI();

            if (!over && vsAI && turn == 2)
                Delay(340, AIMove);
        }

        void AIMove()
        {
            int best = -1, bestScore = int.MinValue;
            for (int i = 0; i < 9; i++)
            {
                if (board[i] != 0) continue;
                board[i] = 2;
                int s = Minimax(board, false);
                board[i] = 0;
                if (s > bestScore) { bestScore = s; best = i; }
            }
            if (best >= 0) DoPlace(best);
        }

        int Minimax(int[] bd, bool maximizing)
        {
            int[] w = FindWin(bd);
            if (w != null) return maximizing ? -10 : 10;
            bool full = true;
            foreach (int x in bd) if (x == 0) { full = false; break; }
            if (full) return 0;

            int best = maximizing ? int.MinValue : int.MaxValue;
            for (int i = 0; i < 9; i++)
            {
                if (bd[i] != 0) continue;
                bd[i] = maximizing ? 2 : 1;
                int v = Minimax(bd, !maximizing);
                bd[i] = 0;
                best = maximizing ? Math.Max(best, v) : Math.Min(best, v);
            }
            return best;
        }

        static int[] FindWin(int[] bd)
        {
            foreach (int[] l in LINES)
                if (bd[l[0]] != 0 && bd[l[0]] == bd[l[1]] && bd[l[0]] == bd[l[2]])
                    return l;
            return null;
        }

        bool IsFull() { foreach (int x in board) if (x == 0) return false; return true; }

        static void Delay(int ms, Action cb)
        {
            Timer t = new Timer { Interval = ms };
            t.Tick += (s, e) => { t.Stop(); t.Dispose(); cb(); };
            t.Start();
        }

        static Label NewLabel(string text, float size, FontStyle fs) => new Label
        {
            Text      = text,
            Font      = new Font("Microsoft JhengHei UI", size, fs),
            AutoSize  = false,
            ForeColor = Color.FromArgb(44, 44, 42)
        };

        static Button NewBtn(string text, Color bg)
        {
            Button b = new Button
            {
                Text      = text,
                Font      = new Font("Microsoft JhengHei UI", 10f),
                FlatStyle = FlatStyle.Flat,
                BackColor = bg,
                ForeColor = Color.White,
                Cursor    = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }
    }

    // ════════════════════════════════════════════════════════
    //  五子棋  Gomoku  (9 / 13 / 15 / 19 路)
    //  棋子落在交叉點
    // ════════════════════════════════════════════════════════
    public class GomokuForm : Form
    {
        int N    = 15;
        int CELL = 36;
        int MAR  = 28;

        int[] board;
        int   turn    = 1;
        bool  over    = false;
        bool  vsAI    = false;
        int[] wins    = new int[3];
        List<int> winLine = new List<int>();
        int lastMove  = -1;

        Panel    pnlBoard;
        Label    lblStatus, lblBlack, lblWhite;
        Button   btnNew, btnMode;
        ComboBox cboSize;

        public GomokuForm()
        {
            Text            = "五子棋 Gomoku";
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox     = false;
            StartPosition   = FormStartPosition.CenterScreen;
            BackColor       = Color.FromArgb(245, 244, 238);
            Font            = new Font("Microsoft JhengHei UI", 10f);

            BuildUI();
            ApplySize();
            NewGame();
        }

        void BuildUI()
        {
            pnlBoard = new Panel
            {
                BackColor = Color.FromArgb(213, 165, 85),
                Cursor    = Cursors.Hand
            };
            pnlBoard.Paint      += PnlBoard_Paint;
            pnlBoard.MouseClick += PnlBoard_Click;
            Controls.Add(pnlBoard);

            lblStatus = NewLabel("", 11, FontStyle.Bold);
            lblBlack  = NewLabel("", 10, FontStyle.Regular);
            lblWhite  = NewLabel("", 10, FontStyle.Regular);

            Label lblSizeHint = new Label
            {
                Text      = "棋盤路數：",
                Font      = new Font("Microsoft JhengHei UI", 10f),
                AutoSize  = true,
                ForeColor = Color.FromArgb(88, 86, 80),
                Name      = "lblSizeHint"
            };

            cboSize = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font          = new Font("Microsoft JhengHei UI", 10f),
                Width         = 90
            };
            cboSize.Items.AddRange(new object[] { "9 路", "13 路", "15 路", "19 路" });
            cboSize.SelectedIndex = 2;
            cboSize.SelectedIndexChanged += (s, e) =>
            {
                int[] sizes = { 9, 13, 15, 19 };
                N = sizes[cboSize.SelectedIndex];
                ApplySize();
                NewGame();
            };

            btnMode = NewBtn("對戰：雙人", Color.FromArgb(60, 52, 137));
            btnMode.Click += (s, e) => { vsAI = !vsAI; NewGame(); };

            btnNew = NewBtn("新局", Color.FromArgb(90, 88, 80));
            btnNew.Click += (s, e) => NewGame();

            Controls.AddRange(new Control[]
                { lblStatus, lblBlack, lblWhite, lblSizeHint, cboSize, btnMode, btnNew });
        }

        void ApplySize()
        {
            if      (N <=  9) { CELL = 52; MAR = 34; }
            else if (N <= 13) { CELL = 40; MAR = 30; }
            else if (N <= 15) { CELL = 36; MAR = 28; }
            else              { CELL = 28; MAR = 24; }

            int boardPx = (N - 1) * CELL + MAR * 2;

            pnlBoard.Location = new Point(14, 14);
            pnlBoard.Size     = new Size(boardPx, boardPx);

            int px = 14 + boardPx + 16;
            int py = 14;

            lblStatus.SetBounds(px, py,        160, 30);
            lblBlack .SetBounds(px, py + 36,   160, 24);
            lblWhite .SetBounds(px, py + 60,   160, 24);

            foreach (Control c in Controls)
                if (c is Label && c.Name == "lblSizeHint")
                    c.SetBounds(px, py + 100, 80, 26);

            cboSize .SetBounds(px, py + 126,   92,  28);
            btnMode .SetBounds(px, py + 170,   148, 36);
            btnNew  .SetBounds(px, py + 216,   148, 36);

            ClientSize = new Size(px + 164, Math.Max(boardPx + 28, 280));
            pnlBoard.Invalidate();
        }

        void NewGame()
        {
            board    = new int[N * N];
            turn     = 1; over = false;
            lastMove = -1;
            winLine.Clear();
            btnMode.Text = vsAI ? "對戰：AI" : "對戰：雙人";
            RefreshUI();
        }

        void RefreshUI()
        {
            lblBlack.Text = "⚫ 黑棋  (勝 " + wins[1] + ")";
            lblWhite.Text = "⚪ 白棋  (勝 " + wins[2] + ")";

            if (over)
                lblStatus.Text = winLine.Count > 0
                    ? (turn == 1 ? "⚫ 黑棋獲勝！" : "⚪ 白棋獲勝！")
                    : "平局！";
            else
                lblStatus.Text = turn == 1 ? "輪到 ⚫ 黑棋" : "輪到 ⚪ 白棋";

            pnlBoard.Invalidate();
        }

        void PnlBoard_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 棋盤線
            Pen linePen = new Pen(Color.FromArgb(150, 108, 40), 1f);
            for (int i = 0; i < N; i++)
            {
                g.DrawLine(linePen, MAR, MAR + i * CELL, MAR + (N - 1) * CELL, MAR + i * CELL);
                g.DrawLine(linePen, MAR + i * CELL, MAR, MAR + i * CELL, MAR + (N - 1) * CELL);
            }
            linePen.Dispose();

            // 外框加粗
            Pen borderPen = new Pen(Color.FromArgb(120, 82, 28), 2f);
            g.DrawRectangle(borderPen, MAR, MAR, (N - 1) * CELL, (N - 1) * CELL);
            borderPen.Dispose();

            // 星位
            DrawStars(g);

            // 棋子（畫在交叉點）
            int pieceR = CELL / 2 - 2;
            for (int r = 0; r < N; r++)
            for (int c = 0; c < N; c++)
            {
                int idx = r * N + c;
                if (board[idx] == 0) continue;
                int cx = MAR + c * CELL;
                int cy = MAR + r * CELL;
                DrawPiece(g, cx, cy, pieceR, board[idx] == 1,
                          winLine.Contains(idx), idx == lastMove);
            }
        }

        void DrawStars(Graphics g)
        {
            List<int[]> stars = new List<int[]>();

            if (N == 9)
            {
                stars.Add(new[] {2,2}); stars.Add(new[] {2,6});
                stars.Add(new[] {4,4});
                stars.Add(new[] {6,2}); stars.Add(new[] {6,6});
            }
            else if (N == 13)
            {
                stars.Add(new[] {3,3});  stars.Add(new[] {3,9});
                stars.Add(new[] {6,6});
                stars.Add(new[] {9,3});  stars.Add(new[] {9,9});
            }
            else if (N == 15)
            {
                stars.Add(new[] {3,3});  stars.Add(new[] {3,7});  stars.Add(new[] {3,11});
                stars.Add(new[] {7,3});  stars.Add(new[] {7,7});  stars.Add(new[] {7,11});
                stars.Add(new[] {11,3}); stars.Add(new[] {11,7}); stars.Add(new[] {11,11});
            }
            else
            {
                stars.Add(new[] {3,3});  stars.Add(new[] {3,9});  stars.Add(new[] {3,15});
                stars.Add(new[] {9,3});  stars.Add(new[] {9,9});  stars.Add(new[] {9,15});
                stars.Add(new[] {15,3}); stars.Add(new[] {15,9}); stars.Add(new[] {15,15});
            }

            SolidBrush sb = new SolidBrush(Color.FromArgb(120, 82, 28));
            foreach (int[] s in stars)
            {
                if (s[0] >= N || s[1] >= N) continue;
                int sx = MAR + s[1] * CELL;
                int sy = MAR + s[0] * CELL;
                g.FillEllipse(sb, sx - 4, sy - 4, 8, 8);
            }
            sb.Dispose();
        }

        void DrawPiece(Graphics g, int cx, int cy, int r, bool black, bool isWin, bool isLast)
        {
            Color fill   = black ? Color.FromArgb(28, 28, 26)  : Color.FromArgb(248, 246, 238);
            Color border = black ? Color.FromArgb(55, 53, 48)  : Color.FromArgb(170, 168, 158);

            SolidBrush br = new SolidBrush(fill);
            g.FillEllipse(br, cx - r, cy - r, r * 2, r * 2);
            br.Dispose();

            Pen bp = new Pen(border, 1.5f);
            g.DrawEllipse(bp, cx - r, cy - r, r * 2, r * 2);
            bp.Dispose();

            if (isWin)
            {
                Pen wp = new Pen(Color.FromArgb(200, 29, 158, 117), 2.5f);
                g.DrawEllipse(wp, cx - r + 1, cy - r + 1, r * 2 - 2, r * 2 - 2);
                wp.Dispose();
            }

            int hr = Math.Max(r / 3, 3);
            SolidBrush hb = new SolidBrush(Color.FromArgb(black ? 50 : 75, 255, 255, 255));
            g.FillEllipse(hb, cx - hr - r / 4, cy - hr - r / 4, hr * 2, hr * 2);
            hb.Dispose();

            if (isLast)
            {
                Color mc = black ? Color.FromArgb(200, 255, 255, 255) : Color.FromArgb(180, 60, 55, 50);
                Pen mp = new Pen(mc, 1.5f);
                g.DrawRectangle(mp, cx - 3, cy - 3, 6, 6);
                mp.Dispose();
            }
        }

        void PnlBoard_Click(object sender, MouseEventArgs e)
        {
            if (over) return;
            float fc = (float)(e.X - MAR) / CELL;
            float fr = (float)(e.Y - MAR) / CELL;
            int c = (int)Math.Round(fc);
            int r = (int)Math.Round(fr);
            if (c < 0 || c >= N || r < 0 || r >= N) return;
            int px = MAR + c * CELL, py = MAR + r * CELL;
            if (Math.Abs(e.X - px) > CELL / 2 || Math.Abs(e.Y - py) > CELL / 2) return;
            DoPlace(r * N + c);
        }

        void DoPlace(int idx)
        {
            if (over || board[idx] != 0) { SE.Invalid(); return; }
            board[idx] = turn;
            lastMove   = idx;
            SE.Place();

            List<int> win = CheckWin(idx, turn);
            if (win != null)
            {
                over = true; winLine = win; wins[turn]++;
                SE.Win();
            }
            else if (IsFull())
            {
                over = true;
                SE.Draw();
            }
            else turn = turn == 1 ? 2 : 1;

            RefreshUI();

            if (!over && vsAI && turn == 2)
                Delay(240, AIMove);
        }

        void AIMove()
        {
            if (over) return;

            List<int> cands = new List<int>();
            for (int i = 0; i < N * N; i++)
            {
                if (board[i] != 0) continue;
                if (HasNeighbor(i, 2)) cands.Add(i);
            }
            if (cands.Count == 0) cands.Add(N / 2 * N + N / 2);

            Random rng = new Random();
            int best = -1;
            long bestScore = long.MinValue;

            foreach (int m in cands)
            {
                board[m] = 2;
                bool aiWin = CheckWin(m, 2) != null;
                board[m] = 0;
                if (aiWin) { DoPlace(m); return; }

                board[m] = 1;
                bool humanWin = CheckWin(m, 1) != null;
                board[m] = 0;
                if (humanWin) { DoPlace(m); return; }

                board[m] = 2;
                long att = ThreatScore(m, 2);
                board[m] = 0;
                board[m] = 1;
                long def = ThreatScore(m, 1);
                board[m] = 0;

                long score = att * 2 + def + rng.Next(4);
                if (score > bestScore) { bestScore = score; best = m; }
            }

            if (best >= 0) DoPlace(best);
        }

        long ThreatScore(int idx, int p)
        {
            int row = idx / N, col = idx % N;
            long total = 0;
            int[][] dirs = { new[]{0,1}, new[]{1,0}, new[]{1,1}, new[]{1,-1} };
            foreach (int[] d in dirs)
            {
                int cnt = 1;
                for (int s = 1; s < 5; s++)
                {
                    int r = row + d[0] * s, c = col + d[1] * s;
                    if (r < 0 || r >= N || c < 0 || c >= N || board[r*N+c] != p) break;
                    cnt++;
                }
                for (int s = 1; s < 5; s++)
                {
                    int r = row - d[0] * s, c = col - d[1] * s;
                    if (r < 0 || r >= N || c < 0 || c >= N || board[r*N+c] != p) break;
                    cnt++;
                }
                total += (long)cnt * cnt * cnt;
            }
            return total;
        }

        bool HasNeighbor(int idx, int range)
        {
            int row = idx / N, col = idx % N;
            for (int dr = -range; dr <= range; dr++)
            for (int dc = -range; dc <= range; dc++)
            {
                if (dr == 0 && dc == 0) continue;
                int r = row + dr, c = col + dc;
                if (r >= 0 && r < N && c >= 0 && c < N && board[r*N+c] != 0)
                    return true;
            }
            return false;
        }

        List<int> CheckWin(int idx, int p)
        {
            int row = idx / N, col = idx % N;
            int[][] dirs = { new[]{0,1}, new[]{1,0}, new[]{1,1}, new[]{1,-1} };
            foreach (int[] d in dirs)
            {
                List<int> line = new List<int> { idx };
                for (int s = 1; s < 5; s++)
                {
                    int r = row + d[0]*s, c = col + d[1]*s;
                    if (r < 0 || r >= N || c < 0 || c >= N || board[r*N+c] != p) break;
                    line.Add(r*N+c);
                }
                for (int s = 1; s < 5; s++)
                {
                    int r = row - d[0]*s, c = col - d[1]*s;
                    if (r < 0 || r >= N || c < 0 || c >= N || board[r*N+c] != p) break;
                    line.Add(r*N+c);
                }
                if (line.Count >= 5) return line;
            }
            return null;
        }

        bool IsFull() { foreach (int x in board) if (x == 0) return false; return true; }

        static void Delay(int ms, Action cb)
        {
            Timer t = new Timer { Interval = ms };
            t.Tick += (s, e) => { t.Stop(); t.Dispose(); cb(); };
            t.Start();
        }

        static Label NewLabel(string text, float size, FontStyle fs) => new Label
        {
            Text      = text,
            Font      = new Font("Microsoft JhengHei UI", size, fs),
            AutoSize  = false,
            ForeColor = Color.FromArgb(44, 44, 42)
        };

        static Button NewBtn(string text, Color bg)
        {
            Button b = new Button
            {
                Text      = text,
                Font      = new Font("Microsoft JhengHei UI", 10f),
                FlatStyle = FlatStyle.Flat,
                BackColor = bg,
                ForeColor = Color.White,
                Cursor    = Cursors.Hand,
                Size      = new Size(148, 36)
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }
    }
}
