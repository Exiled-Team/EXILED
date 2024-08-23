import React from 'react';
import Layout from '@theme/Layout';
import useDocusaurusContext from '@docusaurus/useDocusaurusContext';

//
//
export default function Home() {
  const {siteConfig} = useDocusaurusContext();
  return (
    <Layout
      title={`${siteConfig.title}`}>

        <div className="main">
            <b className="title">EXILED</b>
            <p className="desc">Low-Level plugin framework for SCP: Secret Laboratory servers.</p>
            <img alt="exiled load" className="vid" src="https://imgur.com/wzxq6AF.gif" />
        </div>

    </Layout>
  );
}
