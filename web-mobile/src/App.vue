<template>
  <router-view />
</template>

<style>
html, body { margin: 0; padding: 0; }
body { background: #f7f8fa; }

/* ------------------------------------------------------------------
   桌面浏览器打开时：显示成“正常手机比例”的界面（390×844，圆角居中，
   内容在手机框内滚动，底部弹层也限定在框内）；
   真机（窄屏 < 520px）不受任何影响，仍是全屏。
   ------------------------------------------------------------------ */
@media (min-width: 520px) {
  html, body { height: 100%; }
  body {
    background: #e6e8ec;
    display: flex;
    align-items: center;
    justify-content: center;
  }

  #app {
    width: 390px;                 /* 手机屏宽度（≈ iPhone 12/13/14） */
    height: min(844px, 92vh);     /* 正常手机比例 19.5:9，窗口矮时自动缩短 */
    background: #f7f8fa;
    border-radius: 28px;
    box-shadow: 0 0 0 1px rgba(0, 0, 0, .06), 0 24px 60px rgba(0, 0, 0, .2);
    overflow-y: auto;             /* 内容在手机框内滚动 */
    overflow-x: hidden;
    transform: translateZ(0);     /* 让内部 fixed 弹层限定在手机框内 */
    scrollbar-width: none;        /* 隐藏滚动条，更像手机 */
  }
  #app::-webkit-scrollbar { width: 0; height: 0; }

  /* AI 问答页原来是 100vh，桌面下改为填满手机框高度 */
  body #app .chat-page { height: 100%; }
}
</style>
