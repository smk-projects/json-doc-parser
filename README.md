# JSON 文件轉換成 JSON字串 #
## 功能說明 ##
於視窗上方輸入 JSON 內容後，按下[輸出JSON]，可轉換成JSON格式的字串

## JSON 文件寫法 ##
1. 以Tab(\t)分隔欄位，共四欄，分別為 JSON屬性名稱、資料型態、是否必填、說明。
2. 用"+"區隔階層，例如第一階沒有"+"號，第二階有一個"+"號，第三階有兩個"+"號，依此類推。
3. 階層順序要打對，例如"+info"的下一階要打"++name"，若打"++++name"會告知輸入錯
誤。
4. 每列一定要有4個欄位，但允許整列空白，程式會直接略過，若不足4欄位會告知輸入錯誤。
5. 可在 Word/EXCEL 表格中打好 JSON 規格，並複製後貼到程式中產出 JSON。
6. 資料型態只吃四種：  
    a. object (物件)  
    b. string (字串)  
    c. number (數字)  
    d. boolean (布林)   
    若為陣列，直接在後面加上"[]"表示之，例如: object[]  

以下為 JSON　文件書寫範例

| JSON 屬性名稱 | 資料型態 | 是否必填 | 說明 |
|--|--|--|--
| data | object | Y | 名稱 |
| +total | number | Y | 總筆數 |
| +range | string | Y | 目前筆數，如"1-10" |
| prevUrl | string | Y | 上一頁連結 |
| nextUrl | string | Y | 下一頁連結 |
| items | object[] | - | 書籍列表 |
| +bookName | string | - | 書籍名稱 |
| +isbn | string | - | ISBN碼 |
| +prices | object[] | - | 價格資訊 |
| ++label | string | - | 價格名稱 |
| ++amount | number | - | 金額 |
| ++remark | string | - | 價格備註 |
| ++descriptions | object | - | 說明內容 |
| +++about | string | - | 書籍介紹 |
| +++directory | string | - | 目錄介紹 |
| +++deliver | string | - | 運送說明 |
||||

這個範例透過本程式能產出下列 JSON：

```JSON
{
  "data": {
    "total": "(number*) 總筆數",
    "range": "(string*) 目前筆數，如\"1-10\""
  },
  "prevUrl": "(string*) 上一頁連結",
  "nextUrl": "(string*) 下一頁連結",
  "items": [
    {
      "bookName": "(string) 書籍名稱",
      "isbn": "(string) ISBN碼",
      "prices": [
        {
          "label": "(string) 價格名稱",
          "amount": "(number) 金額",
          "remark": "(string) 價格備註",
          "descriptions": {
            "about": "(string) 書籍介紹",
            "directory": "(string) 目錄介紹",
            "deliver": "(string) 運送說明"
          }
        }
      ]
    }
  ]
}
```




