# azure-notify-costreport-for-slack-function
## 開発環境
- Windows11
- Visual Studio 2022

## Azure Functionsの作成
- プロジェクトテンプレートはAzure Functionsを使用して、C#（.NET 6)で実装
- 

## 使用ライブラリ
Nugetから以下をインストール。

- Azure.Identity
    - アクセストークン取得に使用
- Slack.Webhooks
    - Slack通知に使用

## 環境変数
以下の設定が必要。

- サブスクリプションID
- マネージドIDのクライアントID
- TimerTrigger設定(CRON式)
- 当月日ごとサービスごとの合計の出力日数
- Slack通知用のURL

設定内容は[インフラのリポジトリ](https://github.com/jinka1997/azure-notify-costreport-for-slack-infra)を参照のこと。

## デプロイ
Github Actionsでデプロイする。ワークフローは以下の通り作成。
事前にインフラのリポジトリを使用して、Azure Functionsを作成しておくこと。

1. Visual Studioで発行を押す（プロジェクトを右クリックする）
1. 発行プロファイルの追加
1. Azureを選択して次へ
1. Azure Function Apps(Linux)を選択して次へ
1. デプロイ先のAzure Functionを選択して次へ
1. GitHub Actionsワークフローを使用したCI/CDを選択して完了
1. GitHub Actionsワークフローのワークフローの編集を押す
1. WORKING_DIRECTORYの修正。`CostReportToSlack`を`src/CostReportToSlack`に修正して保存。
1. ワークフローのymlファイルをコミットしてPush。
1. GitHub Actionsのワークフローが動くので、結果を確認する。