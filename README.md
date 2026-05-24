# 棋牌遊戲集 Board Games Collection

> 視窗程式設計 (II) — 作業二

## 專案簡介

本專案以 **單一 C# 檔案**（`BoardGames.cs`）實作三款經典棋牌遊戲，使用 Windows Forms 開發，不需任何額外套件。

| 遊戲 | 說明 |
|------|------|
| ⚫ 黑白棋 Reversi | 8×8 棋盤，夾住對方棋子翻轉顏色，棋子數多者獲勝 |
| ✕ 井字棋 Tic-Tac-Toe | 3×3，X 或 O 先連成三線者獲勝 |
| ● 五子棋 Gomoku | 可選 9／13／15／19 路棋盤，棋子落在交叉點，先連五子者獲勝 |

## 功能特色

- ✅ 三款遊戲整合在同一個主選單
- ✅ 雙人對戰 / AI 對戰（可切換）
- ✅ 音效：落子、翻棋、獲勝、無效操作（Windows Beep）
- ✅ 計分板（跨局累計勝場）
- ✅ 五子棋支援 9、13、15、19 路棋盤切換
- ✅ 五子棋棋子正確落在線與線的交叉點上
- ✅ 獲勝後高亮顯示連線棋子

## 執行環境

- Windows 10 / 11
- Visual Studio 2022
- .NET Framework 4.7.2
- C# 7.3（不需額外設定）

## 執行說明

1. 下載或 Clone 此專案
   ```
   git clone https://github.com/Aaron66666666/s1131501_林昱綸_Boardgames.git
   ```

2. 用 Visual Studio 2022 開啟 `s1131501_林昱綸_Boardgames.sln`

3. 按 `F5` 執行，主選單會自動開啟

4. 在主選單選擇想玩的遊戲即可開始

## 遊戲截圖

<img width="608" height="739" alt="image" src="https://github.com/user-attachments/assets/bf0dc347-52f9-495c-ab2f-e97ea17837c5" />

黑白棋：

<img width="1010" height="828" alt="image" src="https://github.com/user-attachments/assets/d274ecd6-fccb-4e09-a1f2-62dd09f03e8e" />


井字棋：

<img width="812" height="630" alt="image" src="https://github.com/user-attachments/assets/8b3372c5-67a7-42b6-a7f0-559ec071f50e" />


五子棋：

<img width="1133" height="930" alt="image" src="https://github.com/user-attachments/assets/2a7e99ca-0c87-452a-8fd0-6f06ddb2195b" />

## 專案結構

```
s1131501_林昱綸_Boardgames/
├── BoardGames.cs              # 所有遊戲邏輯與 UI（單一檔案）
├── Program.cs                 # 程式進入點
├── s1131501_林昱綸_Boardgames.csproj
├── s1131501_林昱綸_Boardgames.sln
├── .gitignore
└── README.md
```

## 資料來源

- 黑白棋規則：https://zh.wikipedia.org/wiki/黑白棋
- 五子棋規則：https://zh.wikipedia.org/wiki/五子棋
- 井字棋規則：https://zh.wikipedia.org/wiki/井字棋
- Windows Forms 官方文件：https://docs.microsoft.com/zh-tw/dotnet/desktop/winforms/


