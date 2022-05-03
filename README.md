# Azure IoT Hub Device Application Framework for C# 
[Azure IoT Hub](https://docs.microsoft.com/ja-jp/azure/iot-hub/) に接続するデバイスで必要な共通ロジックを実装し、ユーザーコーディング量を少なくするフレームワークライブラリ。  
- [C# 用ライブラリ](csharp/README.md)  
- C/C++ 用ライブラリ - Coming Soon
- Python 用ライブラリ - Coming Soon


## このリポジトリについて  
フレームワークライブラリの設計・実装のサンプルとして公開。  
「[Essense of Software Design](https://note.com/kae_made/m/m2e74d05de8b0)」 に記載の、様々な基本事項に則って開発を行っている。
ここで公開しているフレームワークライブラリは、https://github.com/kae-made/dtdl-iot-app-generator で公開する自動生成ツールでも利用する。  
2022/5/2 第一版公開時点では、まだ、全てのテストが終わっていない状態での公開である。今後、実行テストや自動生成ツールの成長とともにどの様に変化していくかも興味深いと思われるので、github の履歴参照機能を使って、是非、確認してみてほしい。  
開発は、最初に C# のライブラリから始めている。理由は、以下の二つ。  
- 型付け言語である
- 自動生成と親和性が高い言語体系である

## 設計にあたって  
- アプリケーションは、https://docs.microsoft.com/ja-jp/azure/iot-hub/ で公開されている各言語毎に提供された Device SDK を使って実装する。  
- 様々な実行プラットフォーム、実行形式で使えるよう考慮する。
- Azure IoT Hub を活用したデバイスアプリのロジックで、個別のソリューション、機器に依存せず共通に実装しなければならないものを、フレームワークライブラリ内に実装し、IoTソリューションを構成するデバイスの制御ロジックの実装量を低減する。  
- アプリ形式で使う DeviceClient と、IoT Edge Module 形式で使う ModuleClient は、ほぼ同じメソッドセットを持っているが、その特性上、微妙に異なる。この相違は、極力フレームワークライブラリで吸収し、ユーザーコードは必要最小限の対応を可能とする。
- IoT Plug & Play の定義から自動生成可能なロジックを容易に組み込めるようにする。  
- 手書きによるユーザーコードの追加の容易性も考慮する。  

## 共通コード  
それぞれの IoT ソリューション、機器によらない部分の実装は以下の通り。  
- Azure IoT Hub とのセキュアな接続
- Azure IoT Hub へのメッセージ送信  
- Azure IoT Hub への、センシングデータ等の一定間隔の送信
- Azure IoT Hub からのメッセージ受信と、ユーザーコードへの通知、及び、ACK の送信  
- Device Twins Desired Properties の更新の、ユーザーコードへの通知
- Device Twins Reported Properties の更新通知
- Direct Method の起動受信と、ユーザーコードへの通知、及び、結果の送信
- Device 2 Cloud メッセージングでは送信できない大きなデータの送信  

以上の項目を、フレームワークライブラリの機能として実装する。  

