// @ts-check
// Note: type annotations allow type checking and IDEs autocompletion

const lightCodeTheme = require('prism-react-renderer/themes/github');
const darkCodeTheme = require('prism-react-renderer/themes/dracula');

/** @type {import('@docusaurus/types').Config} */
const config = {
  title: 'EXILED',
  tagline: 'SCP:SL Low Level Plugin Framework',
  url: 'https://exiled-team.github.io',
  baseUrl: '/EXILED/',
  onBrokenLinks: 'warn',
  onBrokenMarkdownLinks: 'warn',
  favicon: 'img/favicon.ico',
  organizationName: 'Exiled-Team',
  projectName: 'EXILED',

  presets: [
    [
      'classic',
      /** @type {import('@docusaurus/preset-classic').Options} */
      ({
        docs: {
          sidebarPath: require.resolve('./sidebars.js'),
          // Please change this to your repo.
          editUrl: 'https://github.com/Exiled-Team/EXILED/tree/master/docs',
        },
        theme: {
          customCss: require.resolve('./src/css/custom.css'),
        },
      }),
    ],
  ],

  themeConfig:
    /** @type {import('@docusaurus/preset-classic').ThemeConfig} */
    ({
      colorMode: {
        defaultMode: 'dark',
      },
      /*announcementBar: {
        id: 'announce_bar',
        content:
            'WIP Documentation',
        backgroundColor: '#20232a',
        textColor: '#fff',
        isCloseable: false,
      },*/
      navbar: {
        title: 'Exiled',
        logo: {
          alt: 'Exiled Logo',
          src: 'img/logo.svg',
        },
        items: [
          {
            type: 'doc',
            docId: 'Installation/Intro',
            position: 'left',
            label: 'Documentation',
          },
          {
            href: 'https://github.com/Exiled-Team/EXILED',
            className: 'header-github-link',
            'aria-label': 'GitHub repository',
            position: 'right',
          },
          {
            href: 'https://discord.gg/exiledreboot',
            className: 'header-discord-link',
            'aria-label': 'Discord server',
            position: 'right',
          },
        ],
      },
      footer: {
        style: 'dark',
        links: [],
        copyright: `Copyright © ${new Date().getFullYear()} Exiled-Team, Inc.`,
      },
      prism: {
        theme: lightCodeTheme,
        darkTheme: darkCodeTheme,
      },
    }),
};

module.exports = config;
