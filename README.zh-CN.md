# ACC DualSense Feedback

> **AI 生成内容警告**
>
> 本仓库及发布说明由 AI 辅助生成，可能存在错误。使用前请自行审查代码、构建产物与设置，风险自负。

## v1.2.1.2 — 踏板阻力模式

在 v1.2.1.1 的基础上增加可切换的踏板阻力模式。

### 新增

- 高级设置中 **“踏板阻力”** 标题右侧新增 **渐进阻力 / 恒定轻阻力** 开关。
- **渐进阻力**保持原版 `0x21` 十区渐进曲线，是默认模式，也是默认预设始终使用的模式。
- **恒定轻阻力**使用 `0x01` 连续阻力与 8 位力度，整个行程都可以比 `0x21` 的最低分区档更轻。
- 该模式同时作用于刹车 L2 和油门 R2 的静止阻力；ABS、抱死、换挡等事件脉冲保持独立。
- **“全部重置”**会恢复渐进阻力；所选模式会保存到设置文件。

### 验证

- 完整测试：**176/176 通过**。
- UI 渲染检查覆盖开关、默认预设、全部重置和蓝色关闭状态。

### 下载

- **英文版：** `ACCDualSenseFeedback-v1.2.1.2-win-x64-portable.zip`
- **简体中文版：** `ACCDualSenseFeedback-v1.2.1.2-win-x64-zh-CN-portable.zip`

### SHA-256

```text
2be5fe45ec6357133e35b02324a9980b8abeb26feb43b5a07d125982877bd3f5  ACCDualSenseFeedback-v1.2.1.2-win-x64-portable.zip
276e05481b8f85200fd0945b09a1f236110f17b34319011c4576e687b2635c8f  ACCDualSenseFeedback-v1.2.1.2-win-x64-zh-CN-portable.zip
```

## v1.2.1.1 — 可调扳机与握把震动频率

在保持 v1.2.1 校准默认值的前提下，增加独立的扳机与握把震动频率调节。

### 新增

- **ABS 脉冲**、**刹车抱死提示**、**升挡冲击**各增加 **扳机震动频率** 和 **握把震动频率** 两个滑块。
- **红线脉冲**、**TC 脉冲**、**车轮空转提示**各增加 **扳机震动频率** 滑块。
- 所有频率滑块使用控制器原始 **0–255** 区间；设为 **0** 会关闭该层震动，但保留扳机基础阻力。
- 频率行支持右键恢复该项的校准默认值。
- 默认值保持原来的固定频率：ABS 15/16、抱死 8/8、升挡 28/32、红线 18、TC 40、空转 7。

### 验证

- 完整测试：**173/173 通过**。

### 下载

- **英文版：** `ACCDualSenseFeedback-v1.2.1.1-win-x64-portable.zip`
- **简体中文版：** `ACCDualSenseFeedback-v1.2.1.1-win-x64-zh-CN-portable.zip`

便携包与校验值见 [GitHub Releases](https://github.com/CocaColor/ACC-DualSense-Feedback/releases) 页面。

## 使用说明

这是一个面向 Assetto Corsa Competizione 的 Windows 便携伴随工具。请先解压 ZIP 再运行 EXE，保持同目录 DLL 完整，并使用 USB 连接的 DualSense 或 DualSense Edge。完整说明见 Release 下载页。
