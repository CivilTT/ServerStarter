import{Q as g}from"./QImg.18695983.js";import{j as m,i as d,k as o,l as v,p as y,c as p,h as F,m as x,g as C,_ as w,n as b,r as B,o as E,q as S,s as j,t as D,u as e,d as r,v as Q,x as A,y as I,z as c}from"./index.6ad0a34c.js";import{D as i}from"./DownloadBtn.ffab9439.js";var k=m({name:"QPage",props:{padding:Boolean,styleFn:Function},setup(s,{slots:n}){const{proxy:{$q:a}}=C(),t=d(v,o);if(t===o)return console.error("QPage needs to be a deep child of QLayout"),o;if(d(y,o)===o)return console.error("QPage needs to be child of QPageContainer"),o;const _=p(()=>{const u=(t.header.space===!0?t.header.size:0)+(t.footer.space===!0?t.footer.size:0);if(typeof s.styleFn=="function"){const f=t.isContainer.value===!0?t.containerHeight.value:a.screen.height;return s.styleFn(u,f)}return{minHeight:t.isContainer.value===!0?t.containerHeight.value-u+"px":a.screen.height===0?u!==0?`calc(100vh - ${u}px)`:"100vh":a.screen.height-u+"px"}}),h=p(()=>`q-page${s.padding===!0?" q-layout-padding":""}`);return()=>F("main",{class:h.value,style:_.value},x(n.default))}}),H="/ServerStarter/assets/titleImg.7cf59f13.png";const l=s=>(A("data-v-a2a80e3e"),s=s(),I(),s),V={class:"row justify-center",style:{"min-height":"inherit"}},$={class:"title_box_full row justify-center items-center"},q={class:"title_box"},N=l(()=>e("h1",{class:"title"},[e("b",null,"Server Starter"),c(" for "),e("b",null,"Minecraft")],-1)),P=l(()=>e("h1",{class:"row justify-center"},[e("span",{class:"title_text"},[c(" - Start Minecraft Java edition server only "),e("strong",{class:"title_text_strong"},"ONE"),c(" click ! - ")])],-1)),z={class:"row justify-center download_button text-bold"},M={class:"row q-gutter-md justify-center"},T=l(()=>e("p",{class:"row justify-center text-bold text-yellow"}," Linux\u7248\u306F\u5E74\u5EA6\u672B\u3054\u308D\u306E\u9577\u671F\u30EA\u30EA\u30FC\u30B9\u7248\u306B\u5408\u308F\u305B\u3066\u516C\u958B\u3057\u307E\u3059\uFF01 ",-1)),K=b({__name:"HomeView",props:{value:{}},setup(s){const n=B("");return E(async()=>{const a=await fetch("https://api.github.com/repos/CivilTT/ServerStarter2/releases/latest");n.value=(await a.json()).name}),(a,t)=>(S(),j(k,{style:{"background-color":"black"}},{default:D(()=>[e("div",V,[r(g,{src:H,style:{opacity:".6"}}),e("div",$,[e("div",q,[N,P,e("p",z," \u304A\u4F7F\u3044\u306E\u30D7\u30E9\u30C3\u30C8\u30D5\u30A9\u30FC\u30E0\u306B\u5408\u308F\u305B\u3066\u30C0\u30A6\u30F3\u30ED\u30FC\u30C9\u3057\u3066\u304F\u3060\u3055\u3044\uFF08\u30D0\u30FC\u30B8\u30E7\u30F3\uFF1A"+Q(n.value)+"\uFF09 ",1),e("div",M,[r(i,{"os-name":"windows"}),r(i,{"os-name":"mac"}),r(i,{disable:"","os-name":"linux"})]),T])])])]),_:1}))}});var G=w(K,[["__scopeId","data-v-a2a80e3e"]]);export{G as default};
