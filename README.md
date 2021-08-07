# TSCutter
通过文件大小估算来剪切TS文件，仅适用于恒定码率的TS文件，如TV录制档. 

如果可以的话请使用VideoReDo等专业软件.


```
TSCutter
  通过文件时长推算大小来切割文件的工具(只适合恒定码率的TS档)

Usage:
  TSCutter [options] <Output>

Arguments:
  <Output>  设置输出文件. 使用 - 可以输出到stdout

Options:
  -i, --input <input>             设置输入文件.
  -D, --input-dur <input-dur>     设置输入文件的长度. [hh:mm:ss]
  -ss, --start-time <start-time>  设置起始时间. [hh:mm:ss]
  -to, --stop-time <stop-time>    设置停止时间. [hh:mm:ss]
  -t, --duration <duration>       设置输出长度. [hh:mm:ss]
  --version                       Show version information
  -?, -h, --help                  Show help and usage information
```

## Examples

自动识别文件长度，裁剪`01:00:00`-`01:02:04`的内容，输出文件自动命名
```
TSCutter -i Record.ts -ss 01:00:00 -to 01:02:04 ""
```
自动识别文件长度，从`01:00:00`开始截取5分钟的内容，指定输出文件
```
TSCutter -i Record.ts -ss 01:00:00 -t 05:00 Output.ts
```
指定文件长度，从`01:00:00`开始截取5分钟的内容，指定输出文件
```
TSCutter -i Record.ts -D 03:00:00 -ss 01:00:00 -t 05:00 Output.ts
```
裁剪并利用`ffmpeg`转换为mp4格式
```
TSCutter -i Record.ts -ss 01:00:00 -t 05:00 - | ffmpeg -i - -c copy Output.mp4
```
