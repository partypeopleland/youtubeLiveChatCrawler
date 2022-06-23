# youtubeLiveChatCrawler

## 簡介

官方的 API 不支援離線之後的聊天室訊息取得，所以若是直播活動結束後需要聊天訊息，就只能使用爬蟲  
本專案參考[chat downloader](https://github.com/xenova/chat-downloader)撰寫 c# 版本爬蟲

一開始的版本是 .netFramework，後續調整為 .NET6 Core

## 主要流程

1. 取得直播網址，爬裡面的初始資料: continuation , apiKey
2. 第一次取得聊天室訊息內容，及下一次的 continuation
3. 依據 continuation 重複取得後續聊天室訊息內容直到沒有聊天訊息為止

## 打包指令
```
 dotnet publish -r win-x64 -p:PublishSingleFile=true --self-contained false
```
> 上述為 windows 環境打包指令，其餘請參閱 [dotnet publish](https://docs.microsoft.com/zh-tw/dotnet/core/tools/dotnet-publish)

## 執行
```
youtubeLiveChat.App.exe <videoId>
```