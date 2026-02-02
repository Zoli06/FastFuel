import React from 'react'

export const MenuLink = ({text, to}: {text: string, to: string}) => {
  return (
      <a href={to}>{ text }</a>
    )
    
}
