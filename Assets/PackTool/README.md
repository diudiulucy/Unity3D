## 打包工具
1. Mono使用即时（JIT）编译，并在运行时按需编译代码。IL2CPP使用提前（AOT）编译并在运行之前编译整个应用程序。
    使用IL2CPP进行构建有助于提高运行速度，并减少包体大小比使用Mono少了8MB
2. Unity20203.0 版本 空包12.2MB（IL2CPP）22.2MB
   单ARMv7 大小6.6MB  老的指令集 效率没有ARM64高 可以去掉减小包体，为了兼容一般还是加上
   单ARM64 大小7.0MB  新的指令集 
   空包app占用内存为99.9MB
3. 分渠道打包工具



