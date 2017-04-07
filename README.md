# Wizards

__:I want to play a wizard VR game!__

__:Say no more.__

![Unity Ver][1]

[1]: https://img.shields.io/badge/Unity%20Version-5.6.0f3-brightgreen.svg

## 一些使用上的建議

* __推薦使用Sourcetree__

  整潔好用的圖形化Git介面，連結在[這裡](https://www.sourcetreeapp.com/)
  
* __.gitignore已經設定好了__
 
  要改也是沒問題啦XD 就記得寫一下

* __將來應該會有很高的機率用到git lfs__

  大家不妨可以去裝一下，或者下載並重新安裝最新版本的git
  
  看一下使用說明就大概知道要怎麼用了，我應該會先寫一個.gitattributes
  
  __記得如果你要新增一個要被納入LFS管理的副檔名，要先改.gitattributes__
  
* __開工的時候記得先開自己的branch，不要動master__

  master是給最後build用的
  
  盡量不要去動到主要scene，如果你要測試甚麼自己開一個新的scene來用用

* __最後merge的時候要記得開pull request__

  只要確認好有做到以下幾件事情
  * 增加/修改過的東西是你有碰過的東西
  * 沒有動到主要的scene
  
    如果你不小心碰到的話可以去compare把你改的部分去掉，保留原始檔
    
  * 沒有出現奇怪的meta檔
  
  如果你都有做好，理論上應該可以automatically merge
  
