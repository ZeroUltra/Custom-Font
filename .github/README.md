# CustomFont

Unity 自定义艺术字 CustomFont

## 原理

  unity其实一直就有一个custom font功能,网上也能找到很多博客.

![img](https://img-blog.csdnimg.cn/20200528204821865.jpg?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L0syMDEzMjAxNA==,size_16,color_FFFFFF,t_70)![点击并拖拽以移动](data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw==)

然后我们只要设置相关信息

有多少个文字,就设置相应size大小

**Index:就是字符十进制索引**

**UV:文字在图片中的UV信息**

**Vert:**垂直大小取决于字符的像素大小，例如 您的字符均为128x128，在Vert Width和Height中分别输入128和–128将得到适当比例的字母。 垂直Y必须为负。

**Advance**:从此字符的原点到下一个字符的原点的所需水平距离,差不多也就是宽了

![img](https://img-blog.csdnimg.cn/20200528204940136.jpg?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L0syMDEzMjAxNA==,size_16,color_FFFFFF,t_70)![点击并拖拽以移动](data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw==)编辑

主要 就是每个字的Index

具体可以看看unity的官方文档和另一个文章

[Unity - Manual: Font assets](https://docs.unity3d.com/Manual/class-Font.html)

例如我们在制作数字(0-9)字体的时候就需要 一张数字图片,然后设置`Character Rects`,文档中说的很清楚,`Index`就是指的当前字的 ASCII码的索引(十进制),例如0=48 1=49…..

如果我们想设置更多的文字,也是可以的,因为Unity支持Unicode,但是我们需要一些步骤,`将文字->16进制->十进制

关于编码可以查看这篇文章:[字符编码笔记：ASCII，Unicode 和 UTF-8 - 阮一峰的网络日志](http://www.ruanyifeng.com/blog/2007/10/ascii_unicode_and_utf-8.html)

代码

```cs
string content = "你好";
    for (int i = 0; i < content.Length; i++)
    {
        var bytes = Encoding.Unicode.GetBytes(content[i].ToString());
        var stringBuilder = new StringBuilder();
        for (var j = 0; j < bytes.Length; j += 2)
        {
            //x2是十六进制 两位如果没有用 0补充
            stringBuilder.AppendFormat("{0:x2}{1:x2}", bytes[j + 1], bytes[j]);
        }
        Debug.Log(stringBuilder.ToString());
        //你->4f60
        //好->597d
        int index = Convert.ToInt32(stringBuilder.ToString(), 16);
        Debug.Log(index);
        //你->20320
        //好->22909
    }
```

或者

```cs
 string str = "你好";
        for (int i = 0; i < str.Length; i++)
        {
            Debug.Log(System.Convert.ToInt32(str[i]));
            //log: 20320
            //log: 22909
        }
```

然后我们把字体的”你”的Index设置成20320即可.

## 需要准备些什么

1. 要一个text文本确保是utf-8格式，里面文字内容
2. 要一个艺术字图片，图片大小随便定义，但是要规整的图片 而且自己要知道每个图片字的长宽

使用说明
---

简单说明:



![](/Img/2018052616135844.png)

![](/Img/customfont.gif)

相关链接:[Unity自定义字体](https://blog.csdn.net/K20132014/article/details/80462509)

