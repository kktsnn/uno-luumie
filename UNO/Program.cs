// See https://aka.ms/new-console-template for more information

using LuumieEngine;
using LuumieEngine.SceneManagement;
using UNO.Assets.Repository;
using UNO.Assets.Routines.Game;
using UNO.Assets.Screens;

// ============= SETUP ==========================

GameController.Repository = new GameRepositoryFS();

// ============= CONSOLE SETUP ===================

Console.CursorVisible = false;
ConsoleHelper.SetCurrentFont(fontSize: (short)(Console.LargestWindowHeight * 16 / 60));
ConsoleHelper.Maximize();

// ============== LUUMIE SETUP ===================

LuumieManager.FrameRate = 10;
SceneManager.ChangeScene(typeof(TitleScreen));

// ============ RUN ==============================
LuumieManager.Start();