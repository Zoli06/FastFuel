import React from 'react'
import './MenuLink.css'

export const MenuLink = ({text, to}: {text: string, to: string}) => {
  return (
      <a href={to} className='menu-link'>{ text }</a>
    )
    
}
